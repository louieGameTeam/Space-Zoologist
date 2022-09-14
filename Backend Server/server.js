const createError = require('http-errors');
const path = require('path');
const cookieParser = require('cookie-parser');
const express = require('express');
const keys = require('./config/keys.js');
const compression = require('compression');
const helmet = require('helmet');
const app = express();
const bodyParser = require('body-parser');

// Set up view engine
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');

// Set up middlewares
app.use(bodyParser.urlencoded({ extended: false }));
app.use(express.json());
app.use(compression());
app.use(helmet());
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

// Setting up DB
const mongoose = require('mongoose');
mongoose.connect(keys.mongoURI, {useNewUrlParser: true, useUnifiedTopology: true});

// Setup database models
require('./model/Account');
require('./model/SummaryTrace');
require('./model/SetTrace');

// Setup debugger
const debugInit = require('./debuggers.js').init;

// Setup the routes
require('./routes/index')(app);
require('./routes/authenticationRoutes')(app);
require('./routes/traceRoutes')(app);

// Catch 404 and forward to error handler
app.use(function(req, res, next) {
    next(createError(404));
  });
  
// Error handler
app.use(function(err, req, res, next) {
    // set locals, only providing error in development
    res.locals.message = err.message;
    res.locals.error = req.app.get('env') === 'development' ? err : {};

    // render the error page
    res.status(err.status || 500);
    res.render('error');
});

app.listen(keys.port, () => {
    debugInit("Listening on " + keys.port);
});