//Login form JS
var button = document.getElementById('mainButton');

var openForm = function () {
    button.classList.add('active');
};

var checkInput = function (input) {
    if (input.value.length > 0) {
        input.className = 'active';
    } else {
        input.className = '';
   }
};

var closeForm = function () {
    button.className = '';
};

document.addEventListener("keyup", function (e) {
    if (e.key === "Escape") {
        closeForm();
    }
});

document.addEventListener("keyup", function (e) {
    if (e.key === "Enter") {
        openForm();
    }
});


async function login() {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    const response = await fetch('https://localhost:7190/api/Auth/login', {
        method: 'POST',
        credentials: 'include', // important: includes cookies
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    });

    if (response.ok) {
        closeForm();
        window.location.href = "/Dashboard/Index";

    } else {
        let errorMessage = "Unknown error";
        try {
            const msg = await response.text();
            if (msg) {
                errorMessage = msg;
            }
        } catch (e) {
            console.warn("Could not read error body:", e);
        }
    }

    //if (response.ok) {
    //    document.getElementById('loginMessage').innerText = "✅ Login successful!";
    //    window.location.href = "/Dashboard/Index";
    //} else {
    //    const msg = await response.text();
    //    document.getElementById('loginMessage').innerText = "❌ Login failed: " + msg;
    //}
}

