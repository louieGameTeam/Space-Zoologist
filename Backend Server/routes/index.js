var express = require('express');
var csv = require('csv-express');

var mongoose = require('mongoose');
const SummaryTrace = mongoose.model('summarytraces');
const SetTrace = mongoose.model('settraces');

module.exports = app => {
  /* GET home page. */
  app.get('/', function(req, res, next) {
    SummaryTrace.find({}, function(err, summarytraces) {
      if (err) res.send(err);
      SetTrace.find({}, function(err, settraces) {
        if (err) res.send(err);
          res.render('index', { title: 'Space Zoologist Database', summarytraces: summarytraces, settraces: settraces});
      });
    });
  });

  /* GET export routes. */
  app.get('/exportsummarytraces', function(req, res, next) {
    var filename = "summarytraces.csv";
    SummaryTrace.find().lean().exec({}, function(err, summarytraces) {
      if (err) res.send(err);
  
      res.statusCode = 200;
      res.setHeader('Content-Type', 'text/csv');
      res.setHeader("Content-Disposition", 'attachment; filename='+filename);
      res.csv(summarytraces, true);
    });
  });

  app.get('/exportsettraces', function(req, res, next) {
    var filename = "settraces.csv";
    SetTrace.find().lean().exec({}, function(err, settraces) {
      if (err) res.send(err);
  
      res.statusCode = 200;
      res.setHeader('Content-Type', 'text/csv');
      res.setHeader("Content-Disposition", 'attachment; filename='+filename);
      res.csv(settraces, true);
    });
  });
}
