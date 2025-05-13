console.log("mainpage.js loaded");

if (document.cookie == "" || document.cookie == "logged_out"){
    window.location.href = "/auth";
} 

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hub") 
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function startConnection() {
  try {
    await connection.start();
    checkCookie();
    console.log("SignalR bağlantısı kuruldu.");
  
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
      console.error("Mesaj gönderilemedi:", err); 
      alert("Sunucuya mesaj gönderilemedi: " + err.message);  
    });
}


function sendTestMessage() {

  connection.invoke("HandleMessage", "test")
    .then(() => {
      console.log("Mesaj başarıyla gönderildi.");
    })
    .catch(err => {
      console.error("Mesaj gönderilemedi:", err);  
      alert("Sunucuya mesaj gönderilemedi: " + err.message);  
    });
}




connection.onclose(() => {
  console.log("SignalR bağlantısı kapatıldı.");
});

connection.on("ReceiveMessage", (message) => {
  console.log(message);
  const obj = JSON.parse(message);
  let type = obj["status"];
  let payload = obj["payload"];
  if(type == "logout"){
    console.log("çıkış yapıldı.")
    document.cookie = "";
    window.location.href = "/auth";
  }
  else if (type == "cookieAccepted"){
    console.log("zaten giriş yapılmış!");
  }
  else if(type == "cookieDenied"){
    console.log("bu cookie geçersiz. tekrar giriş yapılmalı.");
    document.cookie = "";
    window.location.href = "/auth";
    
  }

});

function checkCookie() {
  const message = {
      type: "cookieCheckRequest",
      payload: {
          session: document.cookie
        
      }
  };


  connection.invoke("HandleMessage", JSON.stringify(message))
    .then(() => {
    })
    .catch(err => {
    });
}



startConnection();


