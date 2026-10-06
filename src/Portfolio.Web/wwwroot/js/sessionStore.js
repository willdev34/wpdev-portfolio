// Título: sessionStore.js
// Descrição: Leitura e gravação simples no sessionStorage, só com strings (JSInterop sem objetos).
//            Usado para manter a semente do sorteio dos destaques da Home durante a sessão da aba.
//            Se o storage estiver bloqueado (modo privado, política do navegador), nunca lança:
//            a leitura devolve null e a gravação é ignorada, e a Home usa uma semente por carregamento.

(function () {
    window.wpdevSessionGet = function (key) {
        try {
            return window.sessionStorage.getItem(key);
        } catch (e) {
            return null;
        }
    };

    window.wpdevSessionSet = function (key, value) {
        try {
            window.sessionStorage.setItem(key, value);
        } catch (e) {
            // Sem storage: segue sem guardar
        }
    };
})();
