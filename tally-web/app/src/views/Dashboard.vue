<template lang="pug">
Page
  v-ons-card
    .title Select a production source for which you want Tally information.
    .caption Your production engineer will need to be running the OpenTallyBridge tool.

  v-ons-list
    v-ons-list-item(
      v-for="item in instances",
      :key="item.Name",
      modifier="chevron",
      tappable,
      @click="goTo(item['.key'])"
    ) {{ item.Name }}
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
  },
  firebase: {
    instances: db.ref("instances"),
  },
};
</script>
