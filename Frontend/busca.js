// Campo de busca com lista de sugestões, usado no estoque (clientes e produtos) e no
// agendamento pelo balcão. Funciona com mouse e teclado (setas, Enter e Esc).

// Compara sem acentos e sem maiúsculas: "racao" encontra "Ração"
function normalizar(texto) {
    return String(texto ?? "").normalize("NFD").replace(/[̀-ͯ]/g, "").toLowerCase();
}

function escaparBusca(texto) {
    return String(texto ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
}

// Destaca o trecho digitado dentro da sugestão
function realcar(texto, termo) {
    const seguro = escaparBusca(texto);
    if (!termo) return seguro;
    const i = normalizar(texto).indexOf(normalizar(termo));
    if (i < 0) return seguro;
    return escaparBusca(texto.slice(0, i)) + "<mark>" + escaparBusca(texto.slice(i, i + termo.length)) + "</mark>" + escaparBusca(texto.slice(i + termo.length));
}

// buscar(termo) devolve os itens; desenhar(item, termo) devolve o HTML de cada sugestão;
// extra(termo, itens) pode acrescentar uma opção final (ex.: "cadastrar novo").
function criarBusca({ input, lista, buscar, desenhar, escolher, extra }) {
    const campo = document.getElementById(input);
    const ul = document.getElementById(lista);
    let itens = [];
    let ativo = -1;

    function abrir() {
        const termo = campo.value.trim();
        itens = buscar(termo).slice(0, 8);
        const adicional = extra ? extra(termo, itens) : null;
        if (adicional) itens.push(adicional);
        ativo = itens.length ? 0 : -1;
        if (!itens.length) {
            ul.innerHTML = termo ? `<li class="busca-vazio">Nada encontrado para “${escaparBusca(termo)}”.</li>` : "";
        } else {
            ul.innerHTML = itens.map((item, i) => `<li class="busca-item ${item.extra ? "extra" : ""}" role="option" data-i="${i}">${desenhar(item, termo)}</li>`).join("");
        }
        ul.hidden = ul.innerHTML === "";
        campo.setAttribute("aria-expanded", String(!ul.hidden));
        marcar();
    }
    function fechar() { ul.hidden = true; campo.setAttribute("aria-expanded", "false"); }
    function marcar() {
        ul.querySelectorAll(".busca-item").forEach((li, i) => li.classList.toggle("ativo", i === ativo));
        ul.querySelector(".busca-item.ativo")?.scrollIntoView({ block: "nearest" });
    }
    function confirmar(i) {
        const item = itens[i];
        if (!item) return;
        fechar();
        escolher(item, campo.value.trim());
    }

    campo.addEventListener("input", abrir);
    campo.addEventListener("focus", abrir);
    campo.addEventListener("blur", () => setTimeout(fechar, 150));
    campo.addEventListener("keydown", e => {
        if (ul.hidden && (e.key === "ArrowDown" || e.key === "ArrowUp")) { abrir(); return; }
        if (e.key === "ArrowDown") { e.preventDefault(); ativo = Math.min(ativo + 1, itens.length - 1); marcar(); }
        else if (e.key === "ArrowUp") { e.preventDefault(); ativo = Math.max(ativo - 1, 0); marcar(); }
        else if (e.key === "Enter") { e.preventDefault(); if (ativo >= 0) confirmar(ativo); }
        else if (e.key === "Escape") fechar();
    });
    ul.addEventListener("mousedown", e => {
        const li = e.target.closest(".busca-item");
        if (li) { e.preventDefault(); confirmar(Number(li.dataset.i)); }
    });
    return { abrir, fechar };
}
