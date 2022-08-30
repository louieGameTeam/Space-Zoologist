const mongoose = require('mongoose');
const { Schema } = mongoose;

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

mongoose.model('settraces', setTraceSchema);