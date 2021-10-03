<template lang="pug">
Page
  v-ons-card
    .title Select a production source for which you want Tally information.
    .caption Your production engineer will need to be running the OpenTallyBridge tool.
    .caption(style="text-align: right")
      v-ons-button(modifier="quiet", @click="findoutmore") Find Out More
    div(style="clear: both")

  v-ons-list
    v-ons-list-item(
      v-for="item in instances",
      :key="item.Name",
      modifier="chevron",
      tappable,
      @click="goTo(item['.key'])"
    ) {{ item.Name }}
      .right
        .notification {{ item.SourceType }}
</template>

<script>
import { db } from "../db";
import Page from "../components/Page.vue";

export default {
  name: "Dashboard",
  components: { Page },
  data() {
    return {
      instances: [],
    };
  },
  methods: {
    goTo(key) {
      this.$router.push({ path: `/${key}` });
    },
    findoutmore() {
      window.location = "https://tombartindale.github.io/opentally";
    },
  },
  firebase: {
    instances: db.ref("instances"),
  },
};
</script>
