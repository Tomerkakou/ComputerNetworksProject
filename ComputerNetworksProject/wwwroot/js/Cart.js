async function increaseArrow(productId) {
    const res = await fetch(`cart/AddItem?productId=${productId}`)
}

async function decreaseArrow(productId) {
    const res = await fetch(`cart/DecreaseItem?productId=${productId}`)

}


