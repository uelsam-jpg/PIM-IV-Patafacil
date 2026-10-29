// Comportamentos visuais compartilhados por todas as telas do PataFácil:
// transição suave ao trocar de página e sombra na barra superior ao rolar.
(function () {
    const reduzirMovimento = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    // Saída suave: ao clicar num link interno, a página esmaece antes de navegar
    document.addEventListener("click", function (evento) {
        const link = evento.target.closest("a[href]");
        if (!link || reduzirMovimento) return;
        const destino = link.getAttribute("href");
        if (!destino || destino.startsWith("#") || destino.startsWith("javascript:")) return;
        if (link.target === "_blank" || link.hasAttribute("download")) return;
        if (evento.ctrlKey || evento.metaKey || evento.shiftKey || evento.button !== 0) return;
        if (/^(https?:|mailto:|tel:)/i.test(destino)) return;
        if (link.classList.contains("active")) { evento.preventDefault(); return; } // já está nesta tela

        evento.preventDefault();
        document.body.classList.add("saindo");
        setTimeout(() => { window.location.href = link.href; }, 160);
    });

    // Ao voltar pelo histórico do navegador, a página pode vir do cache ainda esmaecida
    window.addEventListener("pageshow", () => document.body.classList.remove("saindo"));

    // A barra superior ganha sombra quando o conteúdo rola por baixo dela
    const barra = document.querySelector(".navbar");
    if (barra) {
        const atualizar = () => barra.classList.toggle("rolou", window.scrollY > 8);
        window.addEventListener("scroll", atualizar, { passive: true });
        atualizar();
    }
})();
