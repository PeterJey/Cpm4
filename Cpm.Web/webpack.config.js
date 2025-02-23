/// <binding AfterBuild='Run - Production' />
var path = require("path");
var WebpackNotifierPlugin = require("webpack-notifier");

module.exports = {
    context: path.join(__dirname, "App"),
    entry: "./app.ts",
    resolve: {
        extensions: [".tsx", ".ts", ".js"]
    },
    output: {
        path: path.join(__dirname, "wwwroot/js"),
        filename: "bundle.js",
        libraryTarget: 'var',
        library: 'App'
    },
    plugins: [
        new WebpackNotifierPlugin()
    ],
    module: {
        loaders: [
            { test: /\.css$/, loader: "style!css" }
        ],
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            }
        ]
    }
};