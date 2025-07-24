document.addEventListener('DOMContentLoaded', () => {
    const token = getCookie('access_token');
    if (!token) return;

    const payload = parseJwt(token);
    if (!payload) return;

    const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    const username = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

    if (username) {
        document.getElementById("usernameDisplay").textContent = username;
    }

    const navLinks = document.getElementById("navbarLinks");
    //const navLinks1 = document.getElementById("navbarLinks1");

    //// Profile page (everyone sees)
    //navLinks1.innerHTML += `
    //    <li class="nav-item">
    //        <a class="nav-link" href="/Dashboard/Index">Profile</a>
    //    </li>
    //`;

    // Only show Manage Employees if role is HR or Admin
    if (role === "hr" || role === "admin") {
        navLinks.innerHTML += `
            <li class="nav-item">
                <a class="nav-link text-dark" href="/EmployeeManagement/Employee/Index">Manage Employees</a>
            </li>
        `;
    }
});

// Helper to read cookies
function getCookie(name) {
    const cookie = document.cookie.split('; ').find(c => c.startsWith(name + '='));
    return cookie ? decodeURIComponent(cookie.split('=')[1]) : null;
}

// Helper to decode JWT
function parseJwt(token) {
    try {
        const payload = token.split('.')[1];
        return JSON.parse(atob(payload));
    } catch {
        return null;
    }
}
