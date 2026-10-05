// Título: skillNetwork.js
// Descrição: Rede animada de habilidades da timeline. Lê as bolhas do DOM, oscila cada uma em volta da
// posição de repouso e liga cada bolha ao ponto do evento com uma linha reta de SVG.
//
// API global (chamada pelo Timeline.razor, só com tipos primitivos):
//   setupSkillNetwork()   idempotente: limpa o estado anterior e reconstrói a partir do DOM atual
//   disposeSkillNetwork() para o loop, desfaz observers e listeners e remove as linhas

(function () {
    'use strict';

    var SVG_NS = 'http://www.w3.org/2000/svg';

    // Limites de segurança: o helper C# garante a zona livre para 14 px de oscilação horizontal e
    // 7 px vertical. Mesmo com dado fora do esperado, o movimento nunca passa disso.
    var MAX_AMPLITUDE_X = 14;
    var MAX_AMPLITUDE_Y = 7;

    // Radianos por segundo: um ciclo completo leva de 12 a 17 segundos
    var SPEED_X = 0.52;
    var SPEED_Y = 0.37;

    var LINE_COLOR = '#6C9EA3';
    var LINE_OPACITY = 0.35;
    var LINE_OPACITY_ACTIVE = 0.85;

    var state = null;

    function setup() {
        dispose();

        var networks = document.querySelectorAll('.skill-network');
        if (networks.length === 0) return;

        var s = {
            events: [],
            visible: new Set(),
            rafId: 0,
            layoutQueued: false,
            startTime: 0,
            lastTime: null,
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

        var svg = document.createElementNS(SVG_NS, 'svg');
        svg.setAttribute('aria-hidden', 'true');
        svg.setAttribute('focusable', 'false');
        // Estilo inline: o CSS escopado do Blazor não alcança elementos criados pelo script.
        // Origem do SVG = origem da rede = centro do ponto do evento. Fica abaixo das bolhas.
        svg.style.cssText = 'position:absolute;left:0;top:0;width:1px;height:1px;overflow:visible;pointer-events:none;';
        network.insertBefore(svg, network.firstChild);

        var bubbles = [];
        network.querySelectorAll('.skill-bubble').forEach(function (el) {
            var angle = parseFloat(el.dataset.angle) * Math.PI / 180;
            var distance = parseFloat(el.dataset.distance);
            var line = document.createElementNS(SVG_NS, 'line');
            line.setAttribute('stroke', LINE_COLOR);
            line.setAttribute('stroke-width', '1');
            line.setAttribute('stroke-opacity', String(LINE_OPACITY));
            svg.appendChild(line);

            bubbles.push({
                el: el,
                line: line,
                name: (el.dataset.name || '').toLowerCase(),
                x: Math.cos(angle) * distance,
                y: Math.sin(angle) * distance,
                phase: parseFloat(el.dataset.phase) || 0,
                amplitude: Math.min(Math.abs(parseFloat(el.dataset.amplitude)) || 0, MAX_AMPLITUDE_X),
                halfW: 0,
                halfH: 0
            });
        });

        var evt = { item: item, network: network, svg: svg, bubbles: bubbles };
        item.__skillNetwork = evt;
        return evt;
    }

    // Mede as bolhas (o tamanho depende da fonte e da largura) e redesenha as linhas
    function layout(s) {
        s.layoutQueued = false;
        var desktop = s.desktopQuery.matches;
        var t = s.reducedQuery.matches ? null : s.lastTime;
        s.events.forEach(function (evt) {
            evt.svg.style.display = desktop ? '' : 'none';
            if (!desktop) {
                evt.bubbles.forEach(function (b) { b.el.style.transform = ''; });
                return;
            }
            evt.bubbles.forEach(function (b) {
                b.halfW = b.el.offsetWidth / 2;
                b.halfH = b.el.offsetHeight / 2;
            });
            drawFrame(evt, t);
        });
    }

    function queueLayout(s) {
        if (s.layoutQueued) return;
        s.layoutQueued = true;
        requestAnimationFrame(function () { if (state === s) layout(s); });
    }

    // Posiciona bolhas e linhas no instante t (segundos). Com t nulo, desenha em repouso.
    function drawFrame(evt, t) {
        evt.bubbles.forEach(function (b) {
            var dx = 0, dy = 0;
            if (t != null && b.amplitude > 0) {
                dx = clamp(b.amplitude * Math.sin(t * SPEED_X + b.phase), MAX_AMPLITUDE_X);
                dy = clamp(b.amplitude * 0.5 * Math.cos(t * SPEED_Y + b.phase), MAX_AMPLITUDE_Y);
                b.el.style.transform = 'translate(calc(-50% + ' + dx.toFixed(2) + 'px), calc(-50% + ' + dy.toFixed(2) + 'px))';
            } else {
                b.el.style.transform = '';
            }
            placeLine(b, b.x + dx, b.y + dy);
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

    function clamp(v, max) {
        return Math.max(-max, Math.min(max, v));
    }

    // Decide entre loop, quadro estático ou nada, conforme viewport, movimento reduzido e visibilidade
    function update(s) {
        if (state !== s) return;
        var animate = s.desktopQuery.matches && !s.reducedQuery.matches && s.visible.size > 0;

        if (!animate) {
            if (s.rafId) {
                cancelAnimationFrame(s.rafId);
                s.rafId = 0;
            }
            s.lastTime = null;
            layout(s); // quadro estático, ou limpeza no mobile
            return;
        }

        if (!s.rafId) {
            s.startTime = performance.now();
            s.rafId = requestAnimationFrame(function tick(now) {
                if (state !== s) return;
                var t = (now - s.startTime) / 1000;
                s.lastTime = t;
                s.visible.forEach(function (evt) { drawFrame(evt, t); });
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
