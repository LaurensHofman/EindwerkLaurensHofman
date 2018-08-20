var firstAddToCart = false;
var cookieCartName = 'shoppingCartRudyCommerce';

var cartProductsElem = document.querySelector('#cart-products');
var cartPriceElem = document.querySelector('#cart-total-price');
var cartPriceSpan = document.querySelector('#totalprice');
var cartDropdown = document.querySelector('#cart-dropdown');


var cartEmptyElem = document.querySelector('#empty-cart');
var cartFilledElem = document.querySelector('#hide-on-empty');

var cartButton = document.querySelector('#shopping-cart');
var cartDropdown = document.querySelector('#cart-dropdown');

var amountOfHoursToSaveCart = 3 ;

var LoadNewDocURL;

var cart = {
    productsElement: cartProductsElem,
    cartFilledElement: cartFilledElem,
    priceElement: cartPriceElem,
    dropdownElement: cartDropdown,
    emptyCartElement: cartEmptyElem,
    priceSpanElement: cartPriceSpan,
    currentCartContent: null,

    displayInitialCart: function () {
        
        this.getCurrentCart();

        // recreates cookie to refresh timer
        createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);

        this.displayCart(this.currentCartContent);
    },

    addToCart: function (event, id, name, price) {

        event.stopPropagation();
        
        this.getCurrentCart();
        
        if (this.currentCartContent == null) {
            this.currentCartContent = {'productList': [{ 'id': id, 'quantity': 1, 'name': name, 'price': price }] };
        }
        else {
            this.currentCartContent = this.addToExistingCart(this.currentCartContent, id, name, price);
        }

        createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);
        this.displayCart(this.currentCartContent);
        
        if (name) {
            if (firstAddToCart == false) {
                // Opening the shopping cart works fine, only if u close it by clicking the shopping cart button itself after opening for the first time.

                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');

                cartButton.click();

                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');

                firstAddToCart = true;
            }
            else {
                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');
            }

        }
    },

    removeOneFromCart: function (event, removeID) {
        var newCartContent = this.getCurrentCart();
        var newQuantity = 0;
        if (this.productIsInCart(this.currentCartContent, removeID)) {
            
            var index = newCartContent.productList.findIndex(p => p.id === removeID);

            if (newCartContent.productList[index].quantity <= 1) {
                // remove
                this.removeFromCart(event, index);
                newQuantity = 0;
            }
            else {
                // minus one
                newCartContent.productList[index].quantity -= 1;
                newQuantity = newCartContent.productList[index].quantity;

                this.currentCartContent = newCartContent;

                createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);
                this.displayCart(this.currentCartContent);
            }
        }

        return newQuantity;
    },

    removeFromCart: function (event, index) {
        this.currentCartContent.productList.splice(index, 1);

        if (this.currentCartContent.productList.length <= 0) {
            eraseCookie(cookieCartName);
        }
        else {
            createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);
            this.displayCart(this.currentCartContent);
        }
    },

    getCurrentCart: function () {

        var cart = readCookie(cookieCartName);
        //var cart = document.cookie;

        if (cart) {
            if (cart != 'undefined') {
                var cartContent = JSON.parse(cart);
            }
        }

        this.currentCartContent = cartContent;

        return cartContent;
    },

    addToExistingCart: function (oc, newID, newName, newPrice) {
        
        var oldCart = oc;

        if (this.productIsInCart(oldCart, newID)) {
            var index = oldCart.productList.findIndex(p => p.id === newID);
            oldCart.productList[index].name = newName;
            oldCart.productList[index].quantity += 1;
        }
        else {
            oldCart.productList.push({ 'id': newID, 'quantity': 1, 'name': newName, 'price': newPrice });
        }

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
            this.cartFilledElement.classList.remove('collapse');
            //this.productsElement.classList.remove('collapse');
            //this.priceElement.classList.remove('collapse');

            this.clearCartDisplay();

            var totalprice = 0;

            var counter = 0;
            var amountOfProducts = 0
            cartContent.productList.forEach(prod => {

                totalprice += this.createListItem(prod, counter);
                counter += 1;
                amountOfProducts += prod.quantity;
            });

            var divAmountOfProds = document.querySelector('#shopping-cart-amount');
            divAmountOfProds.classList.remove('hide');
            divAmountOfProds.innerHTML = amountOfProducts;

            this.priceSpanElement.textContent = totalprice.toFixed(2).replace('.', ',');
        }
        else {
            this.emptyCartElement.classList.remove('collapse');
            this.cartFilledElement.classList.add('collapse');
            //this.dropdownElement.classList.add('p-r-15px');
            //this.productsElement.classList.add('collapse');
            //this.priceElement.classList.add('collapse');
        }
    },

    clearCartDisplay: function () {
        while (this.productsElement.firstChild) {
            this.productsElement.removeChild(this.productsElement.firstChild);
        }
    },

    createListItem: function (product, counter) {
        
        var newLI = document.createElement('li');
        newLI.classList.add('cart-product');

        var newLIRow = document.createElement('div');
        newLIRow.classList.add('row');


        var hiddenID = document.createElement('input');
        hiddenID.id = 'cartItems_' + counter + '__ID';
        hiddenID.name = 'cartItems[' + counter + '].ID';
        hiddenID.type = 'hidden';
        hiddenID.value = product.id;
        newLIRow.appendChild(hiddenID);

        var hiddenQuantity = document.createElement('input');
        hiddenQuantity.id = 'cartItems_' + counter + '__Quantity';
        hiddenQuantity.name = 'cartItems[' + counter + '].Quantity';
        hiddenQuantity.type = 'hidden';
        hiddenQuantity.value = product.quantity;
        newLIRow.appendChild(hiddenQuantity);


        var cartName = document.createElement('div');
        cartName.classList.add('cart-name', 'col-6');
        cartName.textContent = product.name;

        newLIRow.appendChild(cartName);


        var cartQuantity = document.createElement('div');
        cartQuantity.classList.add('cart-quantity', 'col-2');
        cartQuantity.textContent = 'x ' + product.quantity;

        newLIRow.appendChild(cartQuantity);



        var cartPrice = document.createElement('div');
        cartPrice.classList.add('cart-price', 'col-4');

        var unitPrice = product.price;

        var displayPrice = ((unitPrice.replace(',','.')) * product.quantity);

        cartPrice.textContent = '€ ' + displayPrice.toFixed(2).replace('.', ',');

        newLIRow.appendChild(cartPrice);


        newLI.appendChild(newLIRow);

        this.productsElement.appendChild(newLI);

        return displayPrice;
    }
};


//https://www.quirksmode.org/js/cookies.html

function createCookie(name, value, hours) {
    if (hours) {
        var date = new Date();
        date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

window.onload = cart.displayInitialCart();