// Título: skillNetwork.js
// Descrição: Rede animada de habilidades da timeline. Lê as bolhas do DOM, move cada uma de forma suave
// e aleatória dentro de uma zona segura e liga cada bolha ao ponto do evento com uma linha reta de SVG.
//
// API global (chamada pelo Timeline.razor, só com tipos primitivos):
//   setupSkillNetwork()   idempotente: limpa o estado anterior e reconstrói a partir do DOM atual
//   disposeSkillNetwork() para o loop, desfaz observers e listeners e remove as linhas
//
// Zona segura de cada evento (coordenadas com origem no ponto do evento):
//   horizontal: só o lado da rede (oposto ao card), com margem da linha central e da borda do item
//   vertical:   altura do item menos VERTICAL_MARGIN em cima e embaixo, que cobre a data e o card do
//               evento vizinho (o card vizinho fica do mesmo lado da rede). Mesma margem do
//               SkillNetworkLayout.VerticalMargin em C#.
// Cada bolha tem a própria faixa vertical dentro da zona, então duas bolhas do mesmo evento não se
// sobrepõem; uma repulsão leve cobre o caso de nomes que quebram em mais linhas que o previsto.

(function () {
    'use strict';

    var SVG_NS = 'http://www.w3.org/2000/svg';

    var VERTICAL_MARGIN = 40;   // espelha SkillNetworkLayout.VerticalMargin
    var CENTER_MARGIN = 24;     // folga entre a borda da bolha e a linha central
    var EDGE_MARGIN = 8;        // folga até a borda externa do item
    var REPEL_GAP = 6;          // distância mínima desejada entre bordas de bolhas vizinhas

    var MIN_GLIDE_MS = 4000;    // um novo destino a cada 4 a 9 segundos
    var MAX_GLIDE_MS = 9000;
    var MAX_FRAME_MS = 100;     // limita o salto de tempo após aba em segundo plano

    var LINE_COLOR = '#6C9EA3';
    var LINE_OPACITY = 0.35;
    var LINE_OPACITY_ACTIVE = 0.85;

    var state = null;

    // Gerador com semente (mulberry32): o mesmo evento sorteia sempre a mesma sequência
    function seededRandom(seed) {
        var a = seed >>> 0;
        return function () {
            a = (a + 0x6D2B79F5) >>> 0;
            var t = a;
            t = Math.imul(t ^ (t >>> 15), t | 1);
            t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
            return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
        };
    }

    function easeInOutSine(u) {
        return -(Math.cos(Math.PI * u) - 1) / 2;
    }

    function clamp(v, lo, hi) {
        return Math.max(lo, Math.min(hi, v));
    }

    function setup() {
        dispose();

        var networks = document.querySelectorAll('.skill-network');
        if (networks.length === 0) return;

        var s = {
            events: [],
            visible: new Set(),
            rafId: 0,
            layoutQueued: false,
            lastNow: null,
            desktopQuery: window.matchMedia('(min-width: 768px)'),
            reducedQuery: window.matchMedia('(prefers-reduced-motion: reduce)'),
            observer: null,
            listeners: []
        };
        state = s;

        networks.forEach(function (network) {
            s.events.push(buildEvent(network));
        });

        s.observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                var evt = entry.target.__skillNetwork;
                if (!evt) return;
                if (entry.isIntersecting) s.visible.add(evt); else s.visible.delete(evt);
            });
            update(s);
        }, { rootMargin: '100px 0px' });

        s.events.forEach(function (evt) { s.observer.observe(evt.item); });

        listen(s, s.desktopQuery, 'change', function () { update(s); });
        listen(s, s.reducedQuery, 'change', function () { update(s); });
        listen(s, window, 'resize', function () { queueLayout(s); });
        listen(s, document, 'mouseover', function (e) { onHover(s, e, true); });
        listen(s, document, 'mouseout', function (e) { onHover(s, e, false); });

        // A largura das bolhas muda quando a fonte termina de carregar
        if (document.fonts) {
            if (document.fonts.ready) {
                document.fonts.ready.then(function () { if (state === s) queueLayout(s); });
            }
            listen(s, document.fonts, 'loadingdone', function () { queueLayout(s); });
        }

        layout(s);
        update(s);
    }

    function dispose() {
        var s = state;
        if (!s) return;
        state = null;

        if (s.rafId) cancelAnimationFrame(s.rafId);
        if (s.observer) s.observer.disconnect();
        s.listeners.forEach(function (l) { l.target.removeEventListener(l.type, l.handler); });

        s.events.forEach(function (evt) {
            evt.svg.remove();
            delete evt.item.__skillNetwork;
            evt.bubbles.forEach(function (b) {
                b.el.style.transform = '';
                b.el.classList.remove('skill-bubble--match');
            });
        });
    }

    function listen(s, target, type, handler) {
        target.addEventListener(type, handler);
        s.listeners.push({ target: target, type: type, handler: handler });
    }

    function buildEvent(network) {
        var item = network.closest('.timeline-item') || network.parentElement;
        var eventIndex = parseInt(network.dataset.eventIndex, 10) || 0;

        var svg = document.createElementNS(SVG_NS, 'svg');
        svg.setAttribute('aria-hidden', 'true');
        svg.setAttribute('focusable', 'false');
        // Estilo inline: o CSS escopado do Blazor não alcança elementos criados pelo script.
        // Origem do SVG = origem da rede = centro do ponto do evento. Fica abaixo das bolhas.
        svg.style.cssText = 'position:absolute;left:0;top:0;width:1px;height:1px;overflow:visible;pointer-events:none;';
        network.insertBefore(svg, network.firstChild);

        var bubbles = [];
        network.querySelectorAll('.skill-bubble').forEach(function (el, index) {
            var angle = parseFloat(el.dataset.angle) * Math.PI / 180;
            var distance = parseFloat(el.dataset.distance);
            var line = document.createElementNS(SVG_NS, 'line');
            line.setAttribute('stroke', LINE_COLOR);
            line.setAttribute('stroke-width', '1');
            line.setAttribute('stroke-opacity', String(LINE_OPACITY));
            svg.appendChild(line);

            var restX = Math.cos(angle) * distance;
            var restY = Math.sin(angle) * distance;
            // Semente própria por bolha: o sorteio não depende de quando cada uma é desenhada
            var rng = seededRandom((eventIndex + 1) * 1009 + (index + 1) * 7919);
            var phase = (parseFloat(el.dataset.phase) || 0) / (2 * Math.PI); // 0..1, escalona o início

            var b = {
                el: el,
                line: line,
                name: (el.dataset.name || '').toLowerCase(),
                restX: restX,
                restY: restY,
                amplitude: Math.abs(parseFloat(el.dataset.amplitude)) || 0,
                rng: rng,
                halfW: 0, halfH: 0,
                xLo: restX, xHi: restX, yLo: restY, yHi: restY,
                x: restX, y: restY,
                fromX: restX, fromY: restY,
                toX: restX, toY: restY,
                elapsed: 0, duration: 1
            };
            b.duration = MIN_GLIDE_MS + rng() * (MAX_GLIDE_MS - MIN_GLIDE_MS);
            b.elapsed = (phase % 1) * b.duration;
            bubbles.push(b);
        });

        var evt = {
            item: item,
            network: network,
            svg: svg,
            bubbles: bubbles,
            sign: network.dataset.side === 'left' ? -1 : 1
        };
        item.__skillNetwork = evt;
        return evt;
    }

    // Mede as bolhas e o item (dependem da fonte e da largura) e recalcula a zona segura de cada bolha
    function layout(s) {
        s.layoutQueued = false;
        var desktop = s.desktopQuery.matches;
        s.events.forEach(function (evt) {
            evt.svg.style.display = desktop ? '' : 'none';
            if (!desktop) {
                evt.bubbles.forEach(function (b) { b.el.style.transform = ''; });
                return;
            }
            computeZone(evt);
            // Posições atuais e destinos passam a respeitar a zona nova (resize, fonte)
            evt.bubbles.forEach(function (b) {
                b.x = clamp(b.x, b.xLo, b.xHi);
                b.y = clamp(b.y, b.yLo, b.yHi);
                b.fromX = clamp(b.fromX, b.xLo, b.xHi);
                b.fromY = clamp(b.fromY, b.yLo, b.yHi);
                b.toX = clamp(b.toX, b.xLo, b.xHi);
                b.toY = clamp(b.toY, b.yLo, b.yHi);
            });
            render(evt);
        });
    }

    function computeZone(evt) {
        var w = evt.item.offsetWidth;
        var h = evt.item.offsetHeight;
        var n = evt.bubbles.length;
        if (n === 0) return;

        // A origem da rede é o centro do item, então a zona vertical vai de -h/2 a +h/2 menos as margens
        var zoneTop = -h / 2 + VERTICAL_MARGIN;
        var zoneBottom = h / 2 - VERTICAL_MARGIN;
        var laneH = Math.max(0, (zoneBottom - zoneTop) / n);

        evt.bubbles.forEach(function (b, i) {
            b.halfW = b.el.offsetWidth / 2;
            b.halfH = b.el.offsetHeight / 2;

            // Faixa vertical própria: a bolha inteira cabe na faixa, ou fica no centro dela se for maior
            var laneTop = zoneTop + i * laneH;
            var laneBottom = laneTop + laneH;
            var yLo = laneTop + b.halfH;
            var yHi = laneBottom - b.halfH;
            if (yLo > yHi) { yLo = yHi = (laneTop + laneBottom) / 2; }
            b.yLo = yLo;
            b.yHi = yHi;

            // Lado da rede: da linha central (com margem) até a borda do item (com margem)
            var minAbs = b.halfW + CENTER_MARGIN;
            var maxAbs = Math.max(minAbs, w / 2 - b.halfW - EDGE_MARGIN);
            if (evt.sign < 0) { b.xLo = -maxAbs; b.xHi = -minAbs; }
            else { b.xLo = minAbs; b.xHi = maxAbs; }
        });
    }

    function queueLayout(s) {
        if (s.layoutQueued) return;
        s.layoutQueued = true;
        requestAnimationFrame(function () { if (state === s) layout(s); });
    }

    // Avança o movimento da bolha em dt milissegundos: desliza do ponto de partida ao destino com easing
    // e, ao chegar, sorteia um novo destino dentro da zona segura
    function advance(b, dt) {
        b.elapsed += dt;
        while (b.elapsed >= b.duration) {
            b.elapsed -= b.duration;
            b.fromX = b.toX;
            b.fromY = b.toY;
            b.toX = clamp(b.restX + (b.rng() * 2 - 1) * b.amplitude, b.xLo, b.xHi);
            b.toY = clamp(b.restY + (b.rng() * 2 - 1) * b.amplitude, b.yLo, b.yHi);
            b.duration = MIN_GLIDE_MS + b.rng() * (MAX_GLIDE_MS - MIN_GLIDE_MS);
        }
        var e = easeInOutSine(b.elapsed / b.duration);
        b.x = b.fromX + (b.toX - b.fromX) * e;
        b.y = b.fromY + (b.toY - b.fromY) * e;
    }

    // Repulsão leve: se duas bolhas se aproximam, empurra cada uma metade da sobreposição e volta à zona
    function repel(evt) {
        var bs = evt.bubbles;
        for (var i = 0; i < bs.length; i++) {
            for (var j = i + 1; j < bs.length; j++) {
                var a = bs[i], c = bs[j];
                var overlapX = a.halfW + c.halfW + REPEL_GAP - Math.abs(a.x - c.x);
                var overlapY = a.halfH + c.halfH + REPEL_GAP - Math.abs(a.y - c.y);
                if (overlapX <= 0 || overlapY <= 0) continue;

                if (overlapY <= overlapX) {
                    var dirY = a.y <= c.y ? -1 : 1;
                    a.y += dirY * overlapY / 2;
                    c.y -= dirY * overlapY / 2;
                } else {
                    var dirX = a.x <= c.x ? -1 : 1;
                    a.x += dirX * overlapX / 2;
                    c.x -= dirX * overlapX / 2;
                }
                a.x = clamp(a.x, a.xLo, a.xHi); a.y = clamp(a.y, a.yLo, a.yHi);
                c.x = clamp(c.x, c.xLo, c.xHi); c.y = clamp(c.y, c.yLo, c.yHi);
            }
        }
    }

    // Aplica a posição atual: a bolha é deslocada em relação ao repouso e a linha acompanha
    function render(evt) {
        evt.bubbles.forEach(function (b) {
            var dx = b.x - b.restX;
            var dy = b.y - b.restY;
            b.el.style.transform = 'translate(calc(-50% + ' + dx.toFixed(2) + 'px), calc(-50% + ' + dy.toFixed(2) + 'px))';
            placeLine(b, b.x, b.y);
        });
    }

    // Linha reta do ponto até a borda da bolha (o contorno da soft skill continua limpo)
    function placeLine(b, cx, cy) {
        var len = Math.sqrt(cx * cx + cy * cy);
        var ex = cx, ey = cy;
        if (len > 0) {
            var ux = cx / len, uy = cy / len;
            var tx = Math.abs(ux) > 1e-6 ? b.halfW / Math.abs(ux) : Infinity;
            var ty = Math.abs(uy) > 1e-6 ? b.halfH / Math.abs(uy) : Infinity;
            var back = Math.min(tx, ty, len);
            ex = cx - ux * back;
            ey = cy - uy * back;
        }
        b.line.setAttribute('x1', '0');
        b.line.setAttribute('y1', '0');
        b.line.setAttribute('x2', ex.toFixed(1));
        b.line.setAttribute('y2', ey.toFixed(1));
    }

    // Movimento reduzido: volta todas as bolhas ao repouso (já dentro da zona) e para
    function resetToRest(s) {
        s.events.forEach(function (evt) {
            evt.bubbles.forEach(function (b) {
                b.x = b.fromX = b.toX = clamp(b.restX, b.xLo, b.xHi);
                b.y = b.fromY = b.toY = clamp(b.restY, b.yLo, b.yHi);
            });
        });
    }

    // Decide entre loop, quadro estático ou nada, conforme viewport, movimento reduzido e visibilidade
    function update(s) {
        if (state !== s) return;
        var reduced = s.reducedQuery.matches;
        var animate = s.desktopQuery.matches && !reduced && s.visible.size > 0;

        if (!animate) {
            if (s.rafId) {
                cancelAnimationFrame(s.rafId);
                s.rafId = 0;
            }
            s.lastNow = null;
            if (reduced) resetToRest(s);
            layout(s); // quadro estático, ou limpeza no mobile
            return;
        }

        if (!s.rafId) {
            s.lastNow = null;
            s.rafId = requestAnimationFrame(function tick(now) {
                if (state !== s) return;
                var dt = s.lastNow == null ? 0 : Math.min(now - s.lastNow, MAX_FRAME_MS);
                s.lastNow = now;
                s.visible.forEach(function (evt) {
                    evt.bubbles.forEach(function (b) { advance(b, dt); });
                    repel(evt);
                    render(evt);
                });
                s.rafId = requestAnimationFrame(tick);
            });
        }
    }

    // Destaca as bolhas de mesmo nome (sem diferenciar maiúsculas) nos eventos visíveis
    function onHover(s, e, entering) {
        if (!s.desktopQuery.matches) return;
        var el = e.target.closest ? e.target.closest('.skill-bubble') : null;
        if (!el) return;
        var name = (el.dataset.name || '').toLowerCase();

        s.events.forEach(function (evt) {
            if (!s.visible.has(evt)) return;
            evt.bubbles.forEach(function (b) {
                var match = entering && b.name === name;
                b.el.classList.toggle('skill-bubble--match', match);
                b.line.setAttribute('stroke-opacity', String(match ? LINE_OPACITY_ACTIVE : LINE_OPACITY));
            });
        });
    }

    window.setupSkillNetwork = setup;
    window.disposeSkillNetwork = dispose;
})();
