const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: {
        site: './src/js/site.js',
        bootstrap_js: './src/js/bootstrap_js.js',
        validation: './src/js/validation.js',
        gameIndex: './src/js/game/gameIndex.js',
        answerStatement: './src/js/game/answerStatement.js',
        result: './src/js/game/result.js',
        partySelection: './src/js/game/partySelection.js',
        partiesSelection: './src/js/teacher/partiesSelection.js',
        statementResult: './src/js/teacher/statementResult',
        modifyTest: './src/js/teacher/modifyTest.js',
        signalr: './src/js/signalr/signalr.js',
        refreshPage: './src/js/signalr/refreshPage.js',
        teacherGame: './src/js/signalr/teacherGame.js',
        waitForTest: './src/js/signalr/waitForTest.js',
        modifyStatement: './src/js/teacher/modifyStatement.js',
        progressbar: './src/js/shared/progressbar.js',
        startSession: './src/js/teacher/startSession',
        selectStatements: './src/js/teacher/selectStatements.js',
        debateResult: './src/js/game/debateResult.js',
        statementsresult: './src/js/game/statementsResult.js',
        showStatement: './src/js/teacher/showStatement.js',
        waitingScreen: './src/js/game/waitingScreen.js',
        showSessions: './src/js/teacher/showSessions.js',
        customDebateResult: './src/js/teacher/customDebateResult.js',
        listenForRefresh: './src/js/signalr/listenForRefresh.js',
        sendRefreshSignal: './src/js/signalr/sendRefreshSignal.js',
        refreshCodeScreen: './src/js/signalr/refreshCodeScreen.js',
        pastCustomDebateResult: './src/js/teacher/pastCustomDebateResult.js',
        defenitions: './src/js/shared/definitions.js',
        teacherIndex: './src/js/teacher/teacherIndex.js',
        pastPartyResult: './src/js/teacher/pastPartyResult.js',
        partyResult: './src/js/teacher/partyResult.js',
        showParty: './src/js/game/showParty.js'
    },
    output: {
        filename: '[name].entry.js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'dist')
    },
    devtool: 'source-map',
    mode: 'development',
    module: {
        rules: [{test: /\.s?css$/, use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader']},
            {test: /\.eot(\?v=\d+\.\d+\.\d+)?$/, loader: "file-loader"},
            {test: /\.(jpe?g|png|gif)$/i, loader: "file-loader"},
            {test: /\.(woff|woff2)$/, loader: "url-loader?prefix=font/&limit=5000"},
            {test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/, loader: "url-loader?limit=10000&mimetype=application/octet-stream"},
            {test: /\.svg(\?v=\d+\.\d+\.\d+)?$/, loader: "url-loader?limit=10000&mimetype=image/svg+xml"}
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        })
    ]
};
