<template lang="pug">
 v-ons-page
    v-ons-toolbar
      .center OBS Remote Tally

    v-one-list
        ons-list-item(v-for="(instance,id) in instances" v-bind:key="id" tappable modifier="chevron" @click="push(id)")
            //- ons-icon(icon="fa-lock" v-show="instance.Pwd" class="icon") 
            span {{instance.Name}}
</template>

<script>

import { db } from '../db'

export default {
    name: 'Instances',
    data() {
        return {
            instances: {},
    }
  },

  firebase: {
      instances: db.ref('instances'),
  },
  methods: {
    push(id) {
        this.$router.push({path:'/instance/'+id});
    }
  }
}
</script>

<style scoped>

</style>