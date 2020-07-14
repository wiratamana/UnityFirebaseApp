const functions = require('firebase-functions');
const admin = require('firebase-admin');
const utils = require('./utils')
admin.initializeApp(functions.config().firebase);

const db = admin.firestore();
const users = db.collection('users');
const chats = db.collection('chats');

async function generateNewUserUniqueID() {

    const d0 = utils.getRandom3DigitsNumberAsString();  
    const d1 = utils.getRandom3DigitsNumberAsString();  
    const d2 = utils.getRandom3DigitsNumberAsString();  
    const d3 = utils.getRandom3DigitsNumberAsString();  

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
        "FriendList":[],
        "FriendChatRoomIDs":[]
    };

    console.log(`Generated New User Unique ID : ${userUniqueID}`);
    await users.doc(userUniqueID).set(userData);
    return userUniqueID;
});

exports.sendMessage = functions.region("asia-northeast1").https.onCall(async (data, context) => {
    const chatRoomID = data.ChatRoomID;
    const message = data.Message;
    const userUniqueID = data.UserUniqueID;
    const dateUTC = new Date().toUTCString();
    const messageUniqueID = utils.randomValueHex(64);

    // TODO : check if messageUniqueID is duplicate

    const obj = {
        "Message": message,
        "UserUniqueID": userUniqueID,
        "DateUTC": dateUTC,
        "MessageUniqueID": messageUniqueID
    };

    await db.runTransaction(async transaction => {
        const snapshot = await transaction.get(chats.doc(chatRoomID));
        const chatObjects = Array.from(snapshot.data().ChatObjects);
        chatObjects.push(obj);
        transaction.update(chats.doc(chatRoomID), {ChatObjects: chatObjects});
    });

    return obj;  
});