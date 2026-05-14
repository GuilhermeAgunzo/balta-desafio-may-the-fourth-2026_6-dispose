window.disposeBrowser = {
    getItem: function (key) {
        return window.localStorage.getItem(key);
    },
    setItem: function (key, value) {
        window.localStorage.setItem(key, value);
        return true;
    },
    getCurrentPosition: function () {
        return new Promise(function (resolve) {
            if (!navigator.geolocation) {
                resolve(null);
                return;
            }

            navigator.geolocation.getCurrentPosition(
                function (position) {
                    resolve({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude
                    });
                },
                function () {
                    resolve(null);
                },
                {
                    enableHighAccuracy: true,
                    maximumAge: 30000,
                    timeout: 10000
                });
        });
    },
    requestNotificationPermission: async function () {
        if (!("Notification" in window)) {
            return "unsupported";
        }

        return await Notification.requestPermission();
    },
    showNotification: async function (title, body) {
        if (!("Notification" in window) || Notification.permission !== "granted") {
            return false;
        }

        new Notification(title, { body: body, icon: "/favicon.png" });
        return true;
    }
};
