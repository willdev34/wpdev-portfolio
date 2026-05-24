#!/bin/sh

# Substituir URL da API no appsettings.json
# A variável API_BASE_URL é definida no Render como variável de ambiente
if [ -n "$API_BASE_URL" ]; then
    echo "Configurando API URL: $API_BASE_URL"
    sed -i "s|http://localhost:5277/|$API_BASE_URL|g" /usr/share/nginx/html/appsettings.json
else
    echo "API_BASE_URL não definida, usando localhost"
fi

# Iniciar o Nginx
nginx -g "daemon off;"