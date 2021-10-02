import Vue from "vue";
import VueRouter from "vue-router";
import Dashboard from "../views/Dashboard.vue";
import Instance from "../views/Instance.vue";
import Tally from "../views/Tally.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Open Tally",
    component: Dashboard,
    children: [
      {
        path: "about",
        name: "About",
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () =>
          import(/* webpackChunkName: "about" */ "../views/About.vue"),
      },
      {
        path: ":id",
        name: "Instance",
        component: Instance,
        children: [
          {
            path: ":source",
            name: "Tally",
            component: Tally,
          },
        ],
      },
    ],
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

export default router;
