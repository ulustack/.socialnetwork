console.log("auth.js loaded");


const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hub") // Sunucudaki Hub endpoint
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function startConnection() {
  try {
    await connection.start();
    console.log("SignalR bağlantısı kuruldu.");
    //sendLogin("emrekrtl", "232324324");

  } catch (err) {
    console.error("Bağlantı hatası:", err);
    setTimeout(startConnection, 2000);
  }
}



function sendLogin() {
  const message = {
      type: "loginRequest",
      payload: {
          email: document.getElementById("login-email").value,
          password: document.getElementById("login-password").value
        
      }
  };


  connection.invoke("HandleMessage", JSON.stringify(message))
    .then(() => {
      console.log("Mesaj başarıyla gönderildi.");
    })
    .catch(err => {
      console.error("Mesaj gönderilemedi:", err);  // Detaylı hata logları
      alert("Sunucuya mesaj gönderilemedi: " + err.message);  // Hata mesajı kullanıcıya göster
    });
}


function sendTestMessage() {

  connection.invoke("HandleMessage", "test")
    .then(() => {
      console.log("Mesaj başarıyla gönderildi.");
    })
    .catch(err => {
      console.error("Mesaj gönderilemedi:", err);  // Detaylı hata logları
      alert("Sunucuya mesaj gönderilemedi: " + err.message);  // Hata mesajı kullanıcıya göster
    });
}
connection.onclose(() => {
  console.log("SignalR bağlantısı kapatıldı.");
});
startConnection();


