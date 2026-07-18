#!/bin/sh
# ====================================
# Título: generate-sitemap.sh
# Descrição: Gera sitemap.xml em tempo de build, buscando projetos
#            e posts reais na API. Se a API nao responder, gera
#            so as paginas fixas em vez de quebrar o build.
# ====================================

API_URL="${1:-https://wpdev-portfolio-api.onrender.com}"
SITE_URL="${2:-https://wpdev-portfolio-web.onrender.com}"
OUTPUT="${3:-/app/publish/wwwroot/sitemap.xml}"
TODAY=$(date -u +"%Y-%m-%d")

echo "[sitemap] Gerando sitemap.xml (API: $API_URL)"

fetch_json() {
    curl -sf --max-time 10 --retry 3 --retry-delay 5 "$1" || echo "[]"
}

PROJECTS_JSON=$(fetch_json "$API_URL/api/projects")
BLOGPOSTS_JSON=$(fetch_json "$API_URL/api/blogposts")

{
    echo '<?xml version="1.0" encoding="UTF-8"?>'
    echo '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">'

    echo "  <url><loc>${SITE_URL}/</loc><lastmod>${TODAY}</lastmod><changefreq>weekly</changefreq><priority>1.0</priority></url>"
    for path in projects blog timeline now contact; do
        echo "  <url><loc>${SITE_URL}/${path}</loc><lastmod>${TODAY}</lastmod><changefreq>weekly</changefreq><priority>0.8</priority></url>"
    done

    echo "$PROJECTS_JSON" | jq -r '.[] | "\(.id)|\(.createdAt // "")"' 2>/dev/null | while IFS='|' read -r id created; do
        [ -z "$id" ] && continue
        lastmod="${created%%T*}"
        [ -z "$lastmod" ] && lastmod="$TODAY"
        echo "  <url><loc>${SITE_URL}/projects/${id}</loc><lastmod>${lastmod}</lastmod><changefreq>monthly</changefreq><priority>0.6</priority></url>"
    done

    echo "$BLOGPOSTS_JSON" | jq -r '.[] | "\(.id)|\(.publishedAt // "")"' 2>/dev/null | while IFS='|' read -r id published; do
        [ -z "$id" ] && continue
        lastmod="${published%%T*}"
        [ -z "$lastmod" ] && lastmod="$TODAY"
        echo "  <url><loc>${SITE_URL}/blog/${id}</loc><lastmod>${lastmod}</lastmod><changefreq>monthly</changefreq><priority>0.6</priority></url>"
    done

    echo '</urlset>'
} > "$OUTPUT"

echo "[sitemap] Gerado: $(grep -c '<url>' "$OUTPUT") URLs em $OUTPUT"