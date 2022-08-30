const mongoose = require('mongoose');
const SummaryTrace = mongoose.model('summarytraces');
const SetTrace = mongoose.model('settraces');

const debugDB = require('../debuggers.js').db;

module.exports = app => {

    app.post('/traces/summarytrace/submit', async (req, res) => {
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

    app.post('/traces/summarytrace/get', async (req, res) => {
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
}