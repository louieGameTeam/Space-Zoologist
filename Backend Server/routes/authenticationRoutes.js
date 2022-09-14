const mongoose = require('mongoose');
const Account = mongoose.model('accounts');

const argon2i = require('argon2-ffi').argon2i;
const crypto = require('crypto');

const debugDB = require('../debuggers.js').db;

module.exports = app => {
    // Routes
    app.post('/account/login', async (req, res) => {

        var response = {};

        const { username, password } = req.body;
        if (username == null || password == null) {
            response.code = 1;
            response.msg = "Invalid credentials.";
            res.send(response);
            return;
        }

        var userAccount = await Account.findOne({ username: username}, 'username password');
        if (userAccount != null) {
            argon2i.verify(userAccount.password, password).then(async (success) => {
                if (success) {
                    userAccount.lastAuthentication = Date.now();
                    await userAccount.save();
                    response.code = 0;
                    response.msg = "Account found.";
                    response.data = ( ({username}) => ({username}) )(userAccount);
                    res.send(response);
                    return;
                } else {
                    response.code = 1;
                    response.msg = "Invalid credentials.";
                    res.send(response);
                    return;
                }
            });
        } else {
            response.code = 1;
            response.msg = "Invalid credentials.";
            res.send(response);
            return;
        }
    });

    app.post('/account/create', async (req, res) => {
        
        var response = {};

        const { username, password } = req.body;
        if (username == null || password == null) {
            response.code = 1;
            response.msg = "Invalid credentials.";
            res.send(response);
            return;
        }

        var userAccount = await Account.findOne({ username: username}, '_id');
        if (userAccount == null) {
            // Create new account.
            debugDB("Creating a new account...");

            crypto.randomBytes(32, function(err, salt) {
                argon2i.hash(password, salt).then(async (hash) => {
                    // Create the account with the hashed password.
                    var newAccount = new Account({
                        username : username,
                        password : hash,
                        salt: salt,
        
                        lastAuthentication : Date.now()
                    });
                    await newAccount.save();
                    response.code = 0;
                    response.msg = "Account found.";
                    response.data = ( ({username}) => ({username}) )(newAccount);
                    res.send(response);
                    return;
                });
            });
        } else {
            response.code = 2;
            response.msg = "Username is already in use.";
            res.send(response);
        }
        return;
    });
}