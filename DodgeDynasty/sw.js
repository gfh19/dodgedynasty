self.addEventListener("push", function (e) {
	try {
		if (e && e.data) {
			const payload = e.data.json();
			e.waitUntil(
				self.registration.showNotification(payload.title, payload)
			);
		}
	}
	catch { }
});

self.addEventListener('notificationclick', function (e) {
	try {
		e.notification.close();
		e.waitUntil(
			clients.matchAll({ type: 'window' }).then(windowClients => {
				if (windowClients && windowClients.length > 0) {
					var client = windowClients[0];
					client.navigate(client.url);
				}
			})
		);
	}
	catch { }
});