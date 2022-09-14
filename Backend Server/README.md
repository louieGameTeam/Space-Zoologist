# SpaceZoologistBackend
Backend for Space Zoologist.

# Quickstart Guide to Build (Dev)
1. Clone this repository.
2. Install node.js if you do not have it. Link here: https://nodejs.org/en/
3. At root of project, run command `npm install` to install all package dependencies.
4. Ask administrator for access to the dev.js file, which should be added to the project under the config folder. This is supposed to be your own local dev config; not to be shared publicly as it contains sensitive information.   
5. At root of project, run command `npm run dev`. This executes a script called 'dev' inside package.json.
6. (Optional) If you wish to run with debug commands, at root of project, run command `DEBUG=* node server.js`.
7. Once running, the server is able to handle requests sent to the local port via the game. Ensure that the target URLs used in the game match either dev or prod; whatever you are using.

# Quickstart to Build (Prod)
1. If you haven't done the above, do so now.
2. Follow instructions to download Heroku CLI (https://devcenter.heroku.com/articles/heroku-cli).
3. Login to Heroku using command `heroku login` with credentials granted by administrator.
4. Ensure that the branch `prod` in this repo contains only shippable code, and your local `prod` branch is up-to-date after pulling from origin.
5. From local `prod` branch, run command `git push heroku prod:main`. The remote website will now be updated.
