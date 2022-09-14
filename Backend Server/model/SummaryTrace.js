const mongoose = require('mongoose');
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

mongoose.model('summarytraces', summaryTraceSchema);