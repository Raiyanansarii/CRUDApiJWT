// tokenManager.js

const REFRESH_INTERVAL = 13 * 60 * 1000; // 13 minutes
let isRefreshing = false;

// === AUTO REFRESH TOKEN ===
async function refreshAccessToken() {
    if (isRefreshing) return;
    isRefreshing = true;

    try {
        const response = await fetch('https://localhost:7190/api/Auth/refresh', {
            method: 'POST',
            credentials: 'include'
        });

        if (response.ok) {
            console.log("✅ Access token refreshed.");
        } else {
            console.warn("⚠️ Refresh failed. Redirecting to login...");
            window.location.href = '/Account/Login'; // redirect to login page
        }
    } catch (err) {
        console.error("❌ Token refresh error:", err);
        window.location.href = '/Account/Login';
    } finally {
        isRefreshing = false;
    }
}

function startAutoRefresh() {
    setInterval(refreshAccessToken, REFRESH_INTERVAL);
}

// === START auto-refresh ===
startAutoRefresh();
