import Vue from 'vue'
import App from './App.vue'
import { rtdbPlugin } from 'vuefire'
// import VueRouter from 'vue-router'
// import Instances from  './components/Instances'
// import Instance from  './components/Instance'
import Tally from  './components/Tally'
import VueOnsen from 'vue-onsenui'

import 'onsenui/css/onsenui.css';
import 'onsenui/css/onsen-css-components.css';


// Vue.use(VueRouter)
Vue.use(VueOnsen)
Vue.config.productionTip = false
/* eslint-disable */

// const router = new VueRouter({
//     routes: [
//         { path: '/', component: Instances},
//         { path: '/instance/:id', component: Instance }
//     ]
// })

Vue.use(rtdbPlugin)

Vue.component('Tally',Tally);

var appSettings = {
    selectedTally:'NONE'
}

new Vue({
    data() {
        return {
          appSettings
        };
      },
  render: h => h(App)
}).$mount('#app')
