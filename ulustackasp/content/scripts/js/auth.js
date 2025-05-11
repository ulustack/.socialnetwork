console.log("auth.js loaded");


const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hub") 
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function startConnection() {
  try {
    await connection.start();
    checkCookie();
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
  if (type == "loginDenied"){
    console.log("giriş yapma reddedildi.")
  }
  else if(type == "loginAccepted"){
    console.log("sunucu giriş isteğini kabul etti ve bir oturum kimliği gönderdi.");
    let sess_id = payload["session"];
    document.cookie = sess_id; 
    console.log("yönlendirme yapın.");
    window.location.href = "/mainpage";
  }
  else if(type == "logout"){
    console.log("çıkış yapıldı.")
    document.cookie = "";
  }
  else if (type == "cookieAccepted"){
    console.log("zaten giriş yapılmış!");
    console.log("lütfen yönlendirme yapın");
    window.location.href = "/mainpage";
  }
  else if(type == "cookieDenied"){
    console.log("bu cookie geçersiz. tekrar giriş yapılmalı.");
    document.cookie = "";
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


