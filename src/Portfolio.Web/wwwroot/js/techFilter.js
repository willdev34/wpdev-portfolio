// Título: techFilter.js
// Descrição: Resumo do filtro de tecnologias em 2 linhas. Mede quantas linhas os chips ocupam e marca
//            data-overflow no contêiner pai, para o CSS mostrar o botão "+". A única chamada vinda do
//            Blazor recebe uma string (id do contêiner) e não devolve nada: só primitivos no JSInterop.

(function () {
    var MAX_ROWS = 2;

    // Posições verticais distintas dos chips = linhas ocupadas. Vale com o contêiner recolhido ou expandido,
    // porque overflow: hidden esconde os chips mas não muda a posição deles.
    function rowTops(container) {
        var tops = [];
        for (var i = 0; i < container.children.length; i++) {
            var top = Math.round(container.children[i].offsetTop);
            if (tops.indexOf(top) === -1) {
                tops.push(top);
            }
        }
        return tops.sort(function (a, b) { return a - b; });
    }

    function update(container) {
        var area = container.parentElement;
        if (!area) { return; }
        area.setAttribute('data-overflow', rowTops(container).length > MAX_ROWS ? 'true' : 'false');
    }

    // Teclado: se o foco cair num chip escondido (3ª linha ou além), expande pelo próprio botão.
    function onFocusIn(container, event) {
        if (container.classList.contains('filters-chips--expanded')) { return; }

        var chip = event.target;
        while (chip && chip.parentElement !== container) { chip = chip.parentElement; }
        if (!chip) { return; }

        var tops = rowTops(container);
        if (tops.indexOf(Math.round(chip.offsetTop)) < MAX_ROWS) { return; }

        var toggle = container.parentElement.querySelector('.filters-toggle');
        if (!toggle) { return; }

        toggle.click();
        // O navegador rola o contêiner recolhido para mostrar o chip focado; volta ao topo ao expandir
        container.scrollTop = 0;
        requestAnimationFrame(function () { container.scrollTop = 0; });
    }

    window.wpdevSetupTechFilter = function (containerId) {
        var container = document.getElementById(containerId);
        if (!container) { return; }

        update(container);

        if (container.getAttribute('data-tech-filter-ready') === 'true') { return; }
        container.setAttribute('data-tech-filter-ready', 'true');

        container.addEventListener('focusin', function (event) { onFocusIn(container, event); });

        if (typeof ResizeObserver === 'function') {
            new ResizeObserver(function () { update(container); }).observe(container);
        } else {
            window.addEventListener('resize', function () { update(container); });
        }

        // A fonte web muda a largura dos chips depois do primeiro render
        if (document.fonts && document.fonts.ready) {
            document.fonts.ready.then(function () { update(container); });
        }
    };
})();
