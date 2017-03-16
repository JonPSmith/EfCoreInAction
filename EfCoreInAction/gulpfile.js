/// <binding BeforeBuild='pre-build' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp')
    , git = require('git-rev-sync')
    , file = require('gulp-file');

var paths = {
    webroot: "./wwwroot/"
};

gulp.task("pre-build", function () {
    var branchName = git.branch();
    console.log(branchName);
    return file('GitBranchName.txt', branchName, { src: true })
    .pipe(gulp.dest(paths.webroot));
});