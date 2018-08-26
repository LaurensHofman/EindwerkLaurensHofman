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

    // Gets called when the page loads
    displayInitialCart: function () {
        // Gets the current cart
        this.getCurrentCart();

        // Recreates cookie to refresh timer
        createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);

        // Displays the cookie in the dropdown
        this.displayCart(this.currentCartContent);
    },

    // Gets called by clicking the add to cart buttons
    addToCart: function (event, id, name, price) {
        // Stops default behaviour. Allows me to open the cart dropdown when a product gets added.
        event.stopPropagation();

        // Gets the current cart
        this.getCurrentCart();

        // If the cart is empty, create a new cart content with a productList containing a single copy of the chosen product
        if (this.currentCartContent == null) {
            this.currentCartContent = {'productList': [{ 'id': id, 'quantity': 1, 'name': name, 'price': price }] };
        }
        // If the cart is not empty, add the product to the existing cart
        else {
            this.currentCartContent = this.addToExistingCart(this.currentCartContent, id, name, price);
        }

        // Create a cookie with the newly added product
        createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);

        // Update the dropdown to show the newly added product
        this.displayCart(this.currentCartContent);

        // If a name is supplied, that means this function is called by the add to cart button.
        // When its called by the + button in the cartOverview, no name is supplied. 
        // We only want to open the cart dropdown when products are added from product lists with the add to cart button
        if (name) {
            if (firstAddToCart == false) {
                // Opening the shopping cart works fine,
                // except when it's the first time you add one, then for some reason the cart dropdown has to be clicked once
                // Afterwards it works fine

                // Adds the classes to show the dropdown
                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');

                // Clicks the cart dropdown button as explained above (this closes the dropdown)
                cartButton.click();

                // Readds the classes to show the dropdown
                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');

                // Now the cart dropdown works as intended

                firstAddToCart = true;
            }
            else {
                // Adds the classes to show the dropdown
                $('.shoppingcartbutton').addClass('show');
                $('#shopping-cart').attr('aria-expanded', true);
                $('#cart-dropdown').addClass('show');
            }

        }
    },

    removeOneFromCart: function (event, removeID) {
        // Gets the current cart content
        var newCartContent = this.getCurrentCart();
        var newQuantity = 0;

        // If the product to be removed is in the cart
        if (this.productIsInCart(this.currentCartContent, removeID)) {

            // Gets the index of the product to be removed
            var index = newCartContent.productList.findIndex(p => p.id === removeID);

            // If the product has a quantity of 1 (or less somehow)
            if (newCartContent.productList[index].quantity <= 1) {
                // Remove the whole product from the cart
                this.removeFromCart(event, index);
                newQuantity = 0;
            }
            // If the product has a quantity of 2 or more
            else {
                // Remove only one from the cart
                newCartContent.productList[index].quantity -= 1;
                newQuantity = newCartContent.productList[index].quantity;

                // Saves the cart content
                this.currentCartContent = newCartContent;

                // Updates the cookie and display the cart
                createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);
                this.displayCart(this.currentCartContent);
            }
        }

        // Return the new quantity
        return newQuantity;
    },

    // Removes the whole product line from the cart
    removeFromCart: function (event, index) {
        // Splices the array (seperates the product to be removed, known by its index, of the array)
        this.currentCartContent.productList.splice(index, 1);

        // If the cart is empty now, erase the cookie
        if (this.currentCartContent.productList.length <= 0) {
            eraseCookie(cookieCartName);
        }
        // If the cart still has products, update the cookie and display the cart
        else {
            createCookie(cookieCartName, JSON.stringify(this.currentCartContent), amountOfHoursToSaveCart);
            this.displayCart(this.currentCartContent);
        }
    },

    // Gets the current cart from the cookie
    getCurrentCart: function () {

        // Get the cart
        var cart = readCookie(cookieCartName);

        // If the cart is empty or doesn't exist...
        if (cart) {
            // ... Also check whethet the cart has the string-literal content of 'undefined'...
            if (cart != 'undefined') {
                // If the cart actually has content, parse the content to cartContent
                var cartContent = JSON.parse(cart);
            }
        }

        // 'Saves' the cart content
        this.currentCartContent = cartContent;

        return cartContent;
    },

    // Adds 1 product to an existing cart
    addToExistingCart: function (oc, newID, newName, newPrice) {

        // Gets the old cart
        var oldCart = oc;

        // If the product is already in the cart...
        if (this.productIsInCart(oldCart, newID)) {
            // Get the index of the product
            var index = oldCart.productList.findIndex(p => p.id === newID);

            // Update the name (in case language was changed)
            oldCart.productList[index].name = newName;

            // Adds 1 to the quantity
            oldCart.productList[index].quantity += 1;
        }
        else {
            // If the product wasn't in the cart yet, add a new product line to the cart
            oldCart.productList.push({ 'id': newID, 'quantity': 1, 'name': newName, 'price': newPrice });
        }

        return oldCart;
    },

    // Checks if the product is already in the cart
    productIsInCart: function (oldCart, productID) {
        // Default value = false
        var found = false;

        // Loops through every product in the list
        oldCart.productList.forEach(prod => {
            // If the ID of the product equals the productID sought after...
            if (prod.id === productID) {
                // Then return found = true
                return found = true;
            }
        });

        // If the product was not found, return found which has a default value of false
        return found;
    },

    // Displays the cart content in the cart dropdown
    displayCart: function (cartContent) {

        // If the cart is not empty
        if (cartContent) {

            // https://stackoverflow.com/questions/979256/sorting-an-array-of-javascript-objects ; this sorts by name

            // Sort the product list by name
            this.currentCartContent.productList.sort(function (a, b) {
                return a.name.localeCompare(b.name)
            });

            // Collapses the div containing the message that the cart is empty
            this.emptyCartElement.classList.add('collapse');

            // Makes the div, that is going to house the displayed products, visible
            this.cartFilledElement.classList.remove('collapse');

            // Clear the div
            this.clearCartDisplay();

            var totalprice = 0;
            var counter = 0;
            var amountOfProducts = 0

            // Loops through every product in the list and displays it
            cartContent.productList.forEach(prod => {
                // Display the product (also returns the totalPrice for that product)
                totalprice += this.createListItem(prod, counter);

                // Counts how many products are in the cart (to display as a badge above the button)
                amountOfProducts += prod.quantity;
            });

            // Shows the amount of products above the cart dropdown button
            var divAmountOfProds = document.querySelector('#shopping-cart-amount');
            divAmountOfProds.classList.remove('hide');
            divAmountOfProds.innerHTML = amountOfProducts;

            // Shows the totalPrice in the appropriate div, and shows it with a comma
            this.priceSpanElement.textContent = totalprice.toFixed(2).replace('.', ',');
        }
        else {
            // If the cart is empty
            // Show the message that the cart is empty again
            this.emptyCartElement.classList.remove('collapse');
            // Hides the div that showed the products
            this.cartFilledElement.classList.add('collapse');
        }
    },

    // Clears the div that shows the products
    clearCartDisplay: function () {
        // If the div that shows the product has a child (first child), remove the first child
        // Until the div has no children anymore
        while (this.productsElement.firstChild) {
            this.productsElement.removeChild(this.productsElement.firstChild);
        }
    },

    // Creates the <li> for the product
    createListItem: function (product, counter) {

        // Create the <li> and give it the appropriate CSS class
        var newLI = document.createElement('li');
        newLI.classList.add('cart-product');

        // Create a <div> that works as a Bootstrap row
        var newLIRow = document.createElement('div');
        newLIRow.classList.add('row');

        // Creates a hidden <input> element, which is used to send values to the Post Method, with a name that the Model Binder understands
        var hiddenID = document.createElement('input');
        hiddenID.id = 'cartItems_' + counter + '__ID';
        hiddenID.name = 'cartItems[' + counter + '].ID';
        hiddenID.type = 'hidden';
        hiddenID.value = product.id;
        newLIRow.appendChild(hiddenID);
        
        // Creates a hidden <input> element, which is used to send values to the Post Method, with a name that the Model Binder understands
        var hiddenQuantity = document.createElement('input');
        hiddenQuantity.id = 'cartItems_' + counter + '__Quantity';
        hiddenQuantity.name = 'cartItems[' + counter + '].Quantity';
        hiddenQuantity.type = 'hidden';
        hiddenQuantity.value = product.quantity;
        newLIRow.appendChild(hiddenQuantity);

        // Creates a <div> that shows the product name, and adds it to the row
        var cartName = document.createElement('div');
        cartName.classList.add('cart-name', 'col-6');
        cartName.textContent = product.name;

        newLIRow.appendChild(cartName);

        // Creates a <div> that shows the product quantity, and adds it to the row
        var cartQuantity = document.createElement('div');
        cartQuantity.classList.add('cart-quantity', 'col-2');
        cartQuantity.textContent = 'x ' + product.quantity;

        newLIRow.appendChild(cartQuantity);

        // Creates a <div> that shows the price
        var cartPrice = document.createElement('div');
        cartPrice.classList.add('cart-price', 'col-4');

        // Because JS cannot calculate with comma's, which is the way the prices were displayed and sent from the view...
        // Replace the comma's by dots so JS can calculate with it
        var unitPrice = product.price;
        var displayPrice = ((unitPrice.replace(',','.')) * product.quantity);

        // After calculating the display price, put the comma's back
        cartPrice.textContent = '€ ' + displayPrice.toFixed(2).replace('.', ',');

        // Add the <div> to the row
        newLIRow.appendChild(cartPrice);

        // Add the row to the <li>
        newLI.appendChild(newLIRow);

        // Add the <li> to the cart dropdown
        this.productsElement.appendChild(newLI);

        // Return the display price, which can be used by other functions
        return displayPrice;
    }
};


//https://www.quirksmode.org/js/cookies.html
// Creates a cookie, with desired name, value and existence duration
function createCookie(name, value, hours) {
    // If an existence duration was supplied
    if (hours) {
        // Get the date that matches the moment it has to expire
        var date = new Date();
        // Because JS works with milliseconds, convert the hours to milliseconds first
        date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
        // Puts the expiration date in a string
        var expires = "; expires=" + date.toGMTString();
    }
    else {
        // If no existence duration was supplied, don't add an expire part to the cookie
        var expires = "";
    }
    // Creates the cookie
    document.cookie = name + "=" + value + expires + "; path=/";
}

// Reads the cookie
function readCookie(name) {
    // Gets the name with an equality sign
    var nameEQ = name + "=";
    // Gets the cookies and splits them into seperate cookies, which were seperated by semicolons
    var cookieArray = document.cookie.split(';');
    // Loop through the array of cookies
    for (var i = 0; i < cookieArray.length; i++) {
        // Gets the seperate cookie of the array
        var cookie = cookieArray[i];
        
        while (cookie.charAt(0) == ' ') {
            // Loops through the seperate cookie, cutting off the leading white spaces
            cookie = cookie.substring(1, cookie.length);
        }

        // Checks if the cookie starts with the name that was supplied
        if (cookie.indexOf(nameEQ) == 0) {
            // If true, this means we have found the cookie with the supplied name
            // Return the value of the cookie (starts after nameEQ, and ends at the end of the cookie)
            return cookie.substring(nameEQ.length, cookie.length);
        }
    }
    // Returns null when nothing was found
    return null;
}

function eraseCookie(name) {
    // Recreates the cookie with the same name, but the expiration date will be in the past, removing the cookie
    createCookie(name, "", -1);
}

// Displays the initial cart
window.onload = cart.displayInitialCart();