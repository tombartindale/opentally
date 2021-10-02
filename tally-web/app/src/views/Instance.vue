<template lang="pug">
Page
  v-ons-list
    v-ons-list-item(
      v-for="(item, index) of sources",
      :key="item",
      modifier="chevron",
      tappable,
      @click="goTo(index)"
    ) {{ item }}
</template>

<script>
import { db } from "../db";
import Page from "../components/Page.vue";

export default {
  name: "Instance",
  data() {
    return {
      sources: {},
    };
  },
  created() {
    this.$rtdbBind(
      "sources",
      db.ref("instances").child(this.$route.params.id).child("Sources")
    );
  },
  methods: {
    goTo(key) {
      this.$router.push({ path: `/${this.$route.params.id}/${key}` });
    },
  },
  components: { Page },
};
</script>
