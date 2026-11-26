const products = [
    { id: 1, name: "Novela entre hojas", category: "libros", description: "Una historia para leer sin apuro.", price: 18500, icon: "▤", color: "#a3b18a", tag: "Favorito" },
    { id: 2, name: "Cuaderno Botánico", category: "utiles", description: "Tapa dura y hojas para todas tus ideas.", price: 9200, icon: "▧", color: "#d9a521", tag: "Nuevo" },
    { id: 3, name: "Mate Raíz", category: "mates", description: "Mate artesanal personalizado a tu estilo.", price: 16800, icon: "◒", color: "#c16e52", tag: "Artesanal" },
    { id: 4, name: "Termo Bosque 1L", category: "termos", description: "Tu compañero para cada aventura.", price: 38500, icon: "♧", color: "#244536", tag: "Más vendido" },
    { id: 5, name: "Pack Calcos Jardín", category: "calcos", description: "Seis stickers para llenar de color tus cosas.", price: 4500, icon: "✦", color: "#e4b85b", tag: "Pack x6" },
    { id: 6, name: "Agenda Semillas", category: "utiles", description: "Organizá tus días con calma y propósito.", price: 12500, icon: "▤", color: "#b8c5a1", tag: "Nuevo" },
    { id: 7, name: "Mate Correntino", category: "mates", description: "Calabaza seleccionada con virola artesanal.", price: 22000, icon: "◒", color: "#9c7354", tag: "Artesanal" },
    { id: 8, name: "Bombilla Alpaca", category: "termos", description: "Diseño clásico para acompañar tu mate.", price: 7800, icon: "♧", color: "#8fa58c", tag: "Clásico" }
];

let cart = JSON.parse(localStorage.getItem("entreHojasCart") || "[]");
const money = new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS", maximumFractionDigits: 0 });

const productGrid = document.getElementById("productGrid");
const cartCount = document.getElementById("cartCount");
const cartItems = document.getElementById("cartItems");
const cartTotal = document.getElementById("cartTotal");
const cartDrawer = document.getElementById("cartDrawer");
const cartOverlay = document.getElementById("cartOverlay");

function renderProducts(filter = "todos") {
    const visibleProducts = filter === "todos" ? products : products.filter(product => product.category === filter);
    productGrid.innerHTML = visibleProducts.map(product => `
        <article class="product-card">
            <div class="product-image" style="background:${product.color}">
                <span aria-hidden="true">${product.icon}</span>
                <span class="product-tag">${product.tag}</span>
            </div>
            <div class="product-info">
                <h3>${product.name}</h3>
                <p class="product-description">${product.description}</p>
                <div class="product-bottom">
                    <span class="product-price">${money.format(product.price)}</span>
                    <button class="add-product" type="button" data-add="${product.id}" aria-label="Agregar ${product.name}">+</button>
                </div>
            </div>
        </article>
    `).join("");
}

function saveCart() {
    localStorage.setItem("entreHojasCart", JSON.stringify(cart));
}

function updateCart() {
    const totalItems = cart.reduce((total, item) => total + item.quantity, 0);
    cartCount.textContent = totalItems;

    if (!cart.length) {
        cartItems.innerHTML = '<p class="empty-cart">Tu carrito está esperando nuevos favoritos.</p>';
        cartTotal.textContent = money.format(0);
        return;
    }

    cartItems.innerHTML = cart.map(item => `
        <div class="cart-item">
            <div class="cart-item-image" style="background:${item.color}">${item.icon}</div>
            <div>
                <div class="cart-item-name">${item.name}</div>
                <div class="cart-item-price">${money.format(item.price)} c/u</div>
                <div class="quantity-control">
                    <button type="button" data-decrease="${item.id}" aria-label="Disminuir cantidad">−</button>
                    <span>${item.quantity}</span>
                    <button type="button" data-increase="${item.id}" aria-label="Aumentar cantidad">+</button>
                </div>
            </div>
            <button class="remove-item" type="button" data-remove="${item.id}" aria-label="Eliminar ${item.name}">×</button>
        </div>
    `).join("");

    const total = cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
    cartTotal.textContent = money.format(total);
}

function addToCart(id) {
    const product = products.find(item => item.id === id);
    const existing = cart.find(item => item.id === id);
    if (existing) existing.quantity += 1;
    else cart.push({ ...product, quantity: 1 });
    saveCart();
    updateCart();
    openCart();
}

function changeQuantity(id, amount) {
    const item = cart.find(product => product.id === id);
    if (!item) return;
    item.quantity += amount;
    if (item.quantity <= 0) cart = cart.filter(product => product.id !== id);
    saveCart();
    updateCart();
}

function openCart() {
    cartDrawer.classList.add("open");
    cartDrawer.setAttribute("aria-hidden", "false");
    cartOverlay.hidden = false;
    document.body.style.overflow = "hidden";
}

function closeCart() {
    cartDrawer.classList.remove("open");
    cartDrawer.setAttribute("aria-hidden", "true");
    cartOverlay.hidden = true;
    document.body.style.overflow = "";
}

productGrid.addEventListener("click", event => {
    const button = event.target.closest("[data-add]");
    if (button) addToCart(Number(button.dataset.add));
});

cartItems.addEventListener("click", event => {
    const increase = event.target.closest("[data-increase]");
    const decrease = event.target.closest("[data-decrease]");
    const remove = event.target.closest("[data-remove]");
    if (increase) changeQuantity(Number(increase.dataset.increase), 1);
    if (decrease) changeQuantity(Number(decrease.dataset.decrease), -1);
    if (remove) {
        cart = cart.filter(item => item.id !== Number(remove.dataset.remove));
        saveCart();
        updateCart();
    }
});

document.querySelectorAll("[data-filter]").forEach(button => {
    button.addEventListener("click", () => {
        document.querySelectorAll("[data-filter]").forEach(item => item.classList.remove("active"));
        button.classList.add("active");
        renderProducts(button.dataset.filter);
    });
});

document.querySelectorAll("[data-category-link]").forEach(link => {
    link.addEventListener("click", () => {
        const filter = link.dataset.categoryLink;
        document.querySelectorAll("[data-filter]").forEach(item => item.classList.toggle("active", item.dataset.filter === filter));
        renderProducts(filter);
    });
});

document.getElementById("openCart").addEventListener("click", openCart);
document.getElementById("closeCart").addEventListener("click", closeCart);
cartOverlay.addEventListener("click", closeCart);
document.addEventListener("keydown", event => { if (event.key === "Escape") closeCart(); });

document.getElementById("menuToggle").addEventListener("click", event => {
    const nav = document.getElementById("navLinks");
    const expanded = event.currentTarget.getAttribute("aria-expanded") === "true";
    event.currentTarget.setAttribute("aria-expanded", String(!expanded));
    nav.classList.toggle("open");
});

document.querySelectorAll("#navLinks a").forEach(link => link.addEventListener("click", () => {
    document.getElementById("navLinks").classList.remove("open");
    document.getElementById("menuToggle").setAttribute("aria-expanded", "false");
}));

document.getElementById("contactForm").addEventListener("submit", event => {
    event.preventDefault();
    document.getElementById("formMessage").textContent = "¡Gracias! Recibimos tu mensaje y te responderemos pronto.";
    event.currentTarget.reset();
});

document.getElementById("checkoutButton").addEventListener("click", () => {
    if (!cart.length) {
        alert("Agregá al menos un producto para continuar.");
        return;
    }
    alert("¡Gracias por elegir Entre Hojas! El checkout estará disponible muy pronto.");
});

renderProducts();
updateCart();
