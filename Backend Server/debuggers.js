const debug = require('debug');

const init = debug('app:init');
const menu = debug('app:menu');
const db = debug('app:database');
const http = debug('app:http');

module.exports = {
  init, menu, db, http
};