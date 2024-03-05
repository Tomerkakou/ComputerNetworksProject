async function addToCartArrow(productId) {
    const res = await fetch(`cart/AddItem?productId=${productId}`)
}