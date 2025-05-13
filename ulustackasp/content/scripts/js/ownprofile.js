console.log("ownprofile.js loaded");


const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5000/hub") 
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function startConnection() {
  try {
    await connection.start();
    fetchOwnProfile();
    console.log("SignalR bağlantısı kuruldu.");

  } catch (err) {
    console.error("Bağlantı hatası:", err);
    setTimeout(startConnection, 2000);
  }
}




connection.on("ReceiveMessage", (message) => {
  console.log(message);
  const obj = JSON.parse(message);
  let type = obj["status"];
  let payload = obj["payload"];
  if (type == "ownProfileResponse"){
    console.log("profil bilgisi gönderildi.");
    console.log(payload);
  }


});

function fetchOwnProfile() {
  const message = {
      type: "fetchOwnProfile",
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


