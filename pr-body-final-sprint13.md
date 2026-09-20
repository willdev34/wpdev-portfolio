## Resumo

Fecha o restante da Sprint 13: conteudo do Grupo 1, redesign dos
cards do Grupo 4, e correcao de um bug visual encontrado durante a
validacao.

## Grupo 1 - Conteudo

- Home: heading do Stack Tecnico atualizado, Angular removido
- Now: texto de "Sobre esta pagina" reescrito
- Footer: slogan oficial da marca + icones de LinkedIn, GitHub e email
- Contato: email atualizado para contato@wpdevbr.com, icones ao lado
  dos links de rede

## Grupo 4 - Dashboard admin

- Cards maiores com imagem de fundo por assunto, gradiente escuro
  sobreposto para legibilidade
- Imagens otimizadas em .webp
- Badge de mensagens nao lidas preservado

## Fix adicional

- Corrige pixelizacao dos dois thumbnails secundarios na Home
  (usavam ThumbnailWidth do Cloudinary, insuficiente para o espaco
  real na tela; trocado para HeroWidth)

## Validacao

Testado via deploy de Preview antes deste PR, incluindo login do
admin e inspecao visual de todas as paginas alteradas.

## Testes

63 unitarios + 42 integracao, todos passando.
