const crypto = require('crypto');

module.exports = {randomValueHex, getRandom3DigitsNumberAsString }

function randomValueHex (len) {
    return crypto.randomBytes(Math.ceil(len/2))
        .toString('hex') // convert to hexadecimal format
        .slice(0,len);   // return required number of characters
}

function getRandom3DigitsNumberAsString () {
    var randomNumber = String(Math.floor(Math.random() * 999) + 1);
    var pad = "000";
    return pad.substring(0, pad.length - randomNumber.length) + randomNumber
}


