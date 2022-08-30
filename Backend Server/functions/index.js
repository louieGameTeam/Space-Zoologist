const functions = require("firebase-functions");

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
// exports.helloWorld = functions.https.onRequest((request, response) => {
//   functions.logger.info("Hello logs!", {structuredData: true});
//   response.send("Hello from Firebase!");
// });

const express = require("express");
const csv = require("csv-express");

const mongoose = require("mongoose");
const { Schema } = mongoose;

const summaryTraceSchema = new Schema({
    // Basic info
    playerID: String,
    // General time metrics
    totalPlayTime: Number,
    tutorialTime: Number,
    level1Time: Number,
    level2Time: Number,
    level3Time: Number,
    level4Time: Number,
    level5Time: Number,
    // Progression metrics
    totalCompletion: Boolean,
    tutorialComplete: Boolean,
    level1Complete: Boolean,
    level2Complete: Boolean,
    level3Complete: Boolean,
    level4Complete: Boolean,
    level5Complete: Boolean,
    // Research metrics
    numResearchTabOpen: Number,
    timeResearchTabOpen: Number,
    numArticlesRead: Number,
    numBookmarksCreated: Number,
    notesResearchTab: String,
    // Observation metrics
    numObservationToolOpen: Number,
    timeObservationToolOpen: Number,
    notesObservationTool: String,
    // Concept metrics
    numResourceRequests: Number,
    numResourceRequestsApproved: Number,
    numResourceRequestsDenied: Number,
    numDrawToolUsed: Number,
    // Testing metrics
    setTraces: [{ type: Schema.Types.ObjectId, ref: 'settraces' }],
});

const SummaryTrace = mongoose.model('summarytraces', summaryTraceSchema);

const setTraceSchema = new Schema({
    playerID: String,
    levelID: Number,
    setID: Number,
    result: String,
    numDays: Number,
    currency: Number,
    failure: String,
    foodScore: Number,
    terrainScore: Number,
});

const SetTrace = mongoose.model('settraces', setTraceSchema);

const server = express();

/* GET home page. */
server.get("/", function(req, res, next) {
  SummaryTrace.find({}, function(err, summarytraces) {
    if (err) res.send(err);
    SetTrace.find({}, function(err, settraces) {
      if (err) res.send(err);
      res.render("index", {title: "Space Zoologist Database", summarytraces: summarytraces, settraces: settraces});
    });
  });
});

/* GET export routes. */
server.get("/exportsummarytraces", function(req, res, next) {
  const filename = "summarytraces.csv";
  SummaryTrace.find().lean().exec({}, function(err, summarytraces) {
    if (err) res.send(err);

    res.statusCode = 200;
    res.setHeader("Content-Type", "text/csv");
    res.setHeader("Content-Disposition", "attachment; filename="+filename);
    res.csv(summarytraces, true);
  });
});

server.get("/exportsettraces", function(req, res, next) {
  const filename = "settraces.csv";
  SetTrace.find().lean().exec({}, function(err, settraces) {
    if (err) res.send(err);

    res.statusCode = 200;
    res.setHeader("Content-Type", "text/csv");
    res.setHeader("Content-Disposition", "attachment; filename="+filename);
    res.csv(settraces, true);
  });
});

server.post('/traces/summarytrace/submit', async (req, res) => {
    debugDB("Received summarytrace post request.");

    var response = {};

    const summarytrace = req.body;
    if (summarytrace == null) {
        response.code = 1;
        response.msg = "Summary not found in request.";
        res.send(response);
        return;
    }

    var userSummaryTrace = await SummaryTrace.findOne({playerID: summarytrace.playerID});
    if (userSummaryTrace != null)
    {
        debugDB("Found existing record, deleting it.");
        await userSummaryTrace.delete();
    }

    debugDB("Creating new summarytrace...");
    var newSummarytrace = new SummaryTrace({
        playerID : summarytrace.playerID,
        totalPlayTime : summarytrace.totalPlayTime,
        tutorialTime : summarytrace.tutorialTime,
        level1Time : summarytrace.level1Time,
        level2Time : summarytrace.level2Time,
        level3Time : summarytrace.level3Time,
        level4Time : summarytrace.level4Time,
        level5Time : summarytrace.level5Time,
        totalCompletion : summarytrace.totalCompletion,
        tutorialComplete : summarytrace.tutorialComplete,
        level1Complete : summarytrace.level1Complete,
        level2Complete : summarytrace.level2Complete,
        level3Complete : summarytrace.level3Complete,
        level4Complete : summarytrace.level4Complete,
        level5Complete : summarytrace.level5Complete,
        numResearchTabOpen : summarytrace.numResearchTabOpen,
        timeResearchTabOpen : summarytrace.timeResearchTabOpen,
        numArticlesRead : summarytrace.numArticlesRead,
        numBookmarksCreated : summarytrace.numBookmarksCreated,
        notesResearchTab : summarytrace.notesResearchTab,
        numObservationToolOpen : summarytrace.numObservationToolOpen,
        timeObservationToolOpen : summarytrace.timeObservationToolOpen,
        notesObservationTool : summarytrace.notesObservationTool,
        numResourceRequests : summarytrace.numResourceRequests,
        numResourceRequestsApproved : summarytrace.numResourceRequestsApproved,
        numResourceRequestsDenied : summarytrace.numResourceRequestsDenied,
        numDrawToolUsed : summarytrace.numDrawToolUsed,
    });

    for (var i = 0; i < summarytrace.setTraces.length; i++) {
        debugDB("Creating set trace...");
        var settrace = summarytrace.setTraces[i];
        var newSettrace = new SetTrace({
            playerID : settrace.playerID,
            levelID : settrace.levelID,
            setID : settrace.setID,
            result : settrace.result,
            numDays : settrace.numDays,
            currency : settrace.currency,
            failure : settrace.failure,
            foodScore : settrace.foodScore,
            terrainScore : settrace.terrainScore,
        });
        await newSettrace.save();
        newSummarytrace.setTraces.push(newSettrace);
    }

    await newSummarytrace.save();
    response.code = 0;
    response.msg = "Summary succesfully submitted.";
    res.send(response);
    return;
});

server.post('/traces/summarytrace/get', async (req, res) => {
    debugDB("Received summarytrace get request.");

    var response = {};

    const { playerID } = req.body;

    if (playerID == null) {
        response.code = 1;
        response.msg = "Player ID not found in request.";
        res.send(response);
        return;
    }

    var userSummaryTrace = await SummaryTrace.findOne({playerID: playerID});
    if (userSummaryTrace == null)
    {
        response.code = 2;
        response.msg = "Player not found in database.";
        res.send(response);
        return;
    }

    response.code = 0;
    response.msg = "Summary trace successfully found for player.";
    response.data = JSON.stringify(userSummaryTrace);
    res.send(response);
    return;
});

server.post('/account/login', async (req, res) => {

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

server.post('/account/create', async (req, res) => {
    
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

exports.app = functions.https.onRequest(server);
