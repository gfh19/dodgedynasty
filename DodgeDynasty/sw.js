self.addEventListener("push", function (e) {
    const data = e.data;
    e.waitUntil(
        self.registration.showNotification('Push Title', {
            body: 'Push Body'
        })
    );
});

self.addEventListener("message", function (e) {
    //const data = e.data.json();
    e.waitUntil(
        self.registration.showNotification('Msg Title', {
            body: 'Msg Body'
        })
    );
});