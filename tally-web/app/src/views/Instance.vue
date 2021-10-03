<template lang="pug">
Page(:title="instance.Name")
  v-ons-list
    v-ons-list-item(modifier="chevron", tappable, @click="goTo('po')") Program Out
    v-ons-list-item(
      v-for="(item, index) of instance.Sources",
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
      // sources: {},
      instance: {},
    };
  },
  created() {
    this.$rtdbBind(
      "instance",
      db.ref("instances").child(this.$route.params.id)
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
