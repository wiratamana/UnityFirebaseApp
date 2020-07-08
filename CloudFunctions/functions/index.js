const functions = require('firebase-functions');
const admin = require('firebase-admin');
admin.initializeApp(functions.config().firebase);

const db = admin.firestore();
const users = db.collection('users');

function getRandomNumberAsString() {
    var randomNumber = String(Math.floor(Math.random() * 999) + 1);
    var pad = "000";
    return pad.substring(0, pad.length - randomNumber.length) + randomNumber
}

async function generateNewUserUniqueID() {

    const d0 = getRandomNumberAsString();  
    const d1 = getRandomNumberAsString();  
    const d2 = getRandomNumberAsString();  
    const d3 = getRandomNumberAsString();  

    const userUniqueID = `${d0}-${d1}-${d2}-${d3}`;

    var newUser = await users.doc(userUniqueID).get();
    console.log(`userUniqueID : ${userUniqueID} | exist ? ${newUser.exists}`);

    if (newUser.exists) {
        return await generateNewUserUniqueID();
    }

    return userUniqueID;
}

exports.createEmptyUser = functions.region("asia-northeast1").https.onCall(async (data, context) => {

    var userUniqueID = await generateNewUserUniqueID();

    const userData = {
        "Username":null,
        "Gender":null,
        "Birthday":null,
        "Score":0,
        "FriendRequest":[],
        "FriendList":[]
    };

    console.log(`Generated New User Unique ID : ${userUniqueID}`);
    await users.doc(userUniqueID).set(userData);
    return userUniqueID;
});
