<template lang="pug">
v-ons-page
  v-ons-toolbar
    v-ons-back-button Back
    img(
      src="favicon.png",
      style="height: 32px; margin-top: 6px; margin-left: 6px",
      v-if="$route.path == '/'"
    )
    .center {{ title || $route.name }}
    .right
      slot(name="settings")
  slot
</template>

<script>
export default {
  name: "Page",
  components: {},
  props: ["title"],
};
</script>
