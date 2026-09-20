## Resumo

Corrige a formatacao de "Foco Atual" e "Projetos em Andamento" na
pagina publica do Now, que estavam em Poppins negrito grande (como
titulo), destoando de "Aprendendo Agora".

## Mudanca

Remove a classe now-block-text--large dos dois blocos, mantendo so
now-block-text (Inter, peso normal), mesma formatacao das outras
secoes da pagina. Feito em dois commits porque a segunda ocorrencia
(Projetos em Andamento) ficou pra tras na primeira tentativa.

## Testes

63 unitarios + 42 integracao, todos passando.
Validado visualmente via deploy de Preview antes deste PR.
