import Vue from "vue";
import App from "./App.vue";
import "onsenui/css/onsenui.css";
import "onsenui/css/onsen-css-components.css";
import { rtdbPlugin } from "vuefire";
import VueRouter from "vue-router";
import router from "./router";
import VueOnsen from "vue-onsenui";

// (3) And plug in the bindings
Vue.use(VueOnsen);
Vue.use(VueRouter);
Vue.use(rtdbPlugin);

Vue.config.productionTip = false;

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");
