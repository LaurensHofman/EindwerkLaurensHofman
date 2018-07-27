var cartProductsElem = document.querySelector('#cart-products');
var cartPriceElem = document.querySelector('#cart-total-price');
var cartPriceSpan = document.querySelector('#totalprice');

var cartEmptyElem = document.querySelector('#empty-cart');

var cartButton = document.querySelector('#shopping-cart');
var cartDropdown = document.querySelector('#cart-dropdown');

var amountOfMinutesToSaveCart = 0.2;

var cart = {
    productsElement: cartProductsElem,
    priceElement: cartPriceElem,
    emptyCartElement: cartEmptyElem,
    priceSpanElement: cartPriceSpan,
    currentCartContent: null,

    displayInitialCart: function () {

        this.getCurrentCart();

        this.displayCart(this.currentCartContent);
    },

    addToCart: function (event, id, name, price) {

        event.stopPropagation();

        this.getCurrentCart();

        if (this.currentCartContent == null) {
            var now = new Date();

            this.currentCartContent = { 'modifiedAt': now, 'productList': [{ 'id': id, 'quantity': 1, 'name': name, 'price': price }] };
        }
        else {
            this.currentCartContent = this.addToExistingCart(this.currentCartContent, id, name, price);
        }

        localStorage.setItem('cart', JSON.stringify(this.currentCartContent));
        this.displayCart(this.currentCartContent);

        if (!cartButton.parentElement.classList.contains('show')) {
            cartButton.click();
        }
    },

    getCurrentCart: function () {

        var cart = localStorage.getItem('cart');
        var cartContent = JSON.parse(cart);

        if (cartContent) {

            var now = new Date();
            var cartDate = new Date(cartContent.modifiedAt);

            if ((Math.floor(now - cartDate) / 60000) > amountOfMinutesToSaveCart) {

                cartContent = null;
                localStorage.removeItem('cart');
            }
        }

        this.currentCartContent = cartContent;
    },

    addToExistingCart: function (oc, newID, newName, newPrice) {
        var oldCart = oc;

        if (this.productIsInCart(oldCart, newID)) {
            var index = oldCart.productList.findIndex(p => p.id === newID);
            oldCart.productList[index].quantity += 1;
        }
        else {
            oldCart.productList.push({ 'id': newID, 'quantity': 1, 'name': newName, 'price': newPrice });
        }

        var now = new Date();

        oldCart.modifiedAt = now;

        return oldCart;
    },

    productIsInCart: function (oldCart, productID) {
        var found = false;

        oldCart.productList.forEach(prod => {
            if (prod.id === productID) {
                return found = true;
            }
        });

        return found;
    },

    displayCart: function (cartContent) {

        if (cartContent) {

            // https://stackoverflow.com/questions/979256/sorting-an-array-of-javascript-objects ; this sorts by name

            this.currentCartContent.productList.sort(function (a, b) {
                return a.name.localeCompare(b.name)
            });

            this.emptyCartElement.classList.add('collapse');
            this.productsElement.classList.remove('collapse');
            this.priceElement.classList.remove('collapse');

            this.clearCartDisplay();

            var totalprice = 0;

            cartContent.productList.forEach(prod => {

                totalprice += this.createListItem(prod);

            });

            this.priceSpanElement.textContent = totalprice.toFixed(2).replace('.', ',');
        }
        else {
            this.emptyCartElement.classList.remove('collapse');
            this.productsElement.classList.add('collapse');
            this.priceElement.classList.add('collapse');
        }
    },

    clearCartDisplay: function () {
        while (this.productsElement.firstChild) {
            this.productsElement.removeChild(this.productsElement.firstChild);
        }
    },

    createListItem: function (product) {

        var newLI = document.createElement('li');
        newLI.classList.add('cart-product');

        var newLIRow = document.createElement('div');
        newLIRow.classList.add('row');



        var cartName = document.createElement('div');
        cartName.classList.add('cart-name', 'col-6');
        cartName.textContent = product.name;

        newLIRow.appendChild(cartName);


        //var cartQuantityButtons = document.createElement('div');
        //cartQuantityButtons.classList.add('cart-quantity-buttons', 'col-1');

        //var plusRow = document.createElement('div');
        //plusRow.classList.add('row');
        //var plus = document.createElement('button');
        //plus.classList.add('btn', 'btn-success');
        //plus.textContent = '+';
        //plusRow.appendChild(plus);

        //var minusRow = document.createElement('div');
        //minusRow.classList.add('row');
        //var minus = document.createElement('button');
        //minus.classList.add('btn', 'btn-danger');
        //minus.textContent = '-';
        //minusRow.appendChild(minus);

        //cartQuantityButtons.appendChild(plusRow);
        //cartQuantityButtons.appendChild(minusRow);

        //newLIRow.appendChild(cartQuantityButtons);



        var cartQuantity = document.createElement('div');
        cartQuantity.classList.add('cart-quantity', 'col-2');
        cartQuantity.textContent = 'x ' + product.quantity;

        newLIRow.appendChild(cartQuantity);



        var cartPrice = document.createElement('div');
        cartPrice.classList.add('cart-price', 'col-4');

        var unitPrice = product.price;

        var displayPrice = (parseFloat(unitPrice) * product.quantity);

        cartPrice.textContent = '€ ' + displayPrice.toFixed(2).replace('.', ',');

        newLIRow.appendChild(cartPrice);


        newLI.appendChild(newLIRow);

        this.productsElement.appendChild(newLI);

        return displayPrice;
    }
};

window.onload = cart.displayInitialCart();
