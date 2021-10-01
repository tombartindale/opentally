// See http://brunch.io for documentation.
exports.files = {
  javascripts: {
    joinTo: {
      "vendor.js": /^(?!app)/, // Files that are not in `app` dir.
      "app.js": /^app/,
    },
  },
  stylesheets: { joinTo: "app.css" },
};

exports.paths = {
  public: "../docs",
};

exports.npm = {
  styles: { highlightjs: ["styles/default.css"] },
};

exports.plugins = {
  babel: { presets: ["latest"] },
  copycat: {
    fonts: ["./node_modules/@fortawesome/fontawesome-free/webfonts"],
  },
  pug: {
    preCompilePattern: /\.html\.pug$/,
  },
};
