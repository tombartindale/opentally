<template lang="pug">
.tally(:class="{ live: liveMe, preview: previewMe }") {{instance.Tally}}
    div(:class="{recording: liveOut}")
</template>

<script>
export default {
    name: 'Tally',
    props:['instance','source'],
    computed:{
        liveOut(){
            return this.instance.Streaming || this.instance.Recording;
        },
        liveMe(){
            if (this.instance && this.instance.Tally)
                return this.instance.Tally.includes(this.$props.source) && (this.instance.Streaming || this.instance.Recording)
            else
                return false;
        },

        previewMe(){
            if (this.instance && this.instance.PreviewTally)
                return this.instance.PreviewTally.includes(this.$props.source)
            else
                return false;
        }
    }
}
</script>

<style lang="stylus" scoped>
.tally {
    border: 10px transparent solid
    height: 100%;
}

.live {
    border: 10px red solid;
    background red
}

.preview
{
    border: 10px red solid;
    animation: flash .7s infinite reverse ease;
}

.recording {
	background: rgba(255, 82, 82, 1);
	border-radius: 50%;
	margin: 10px;
	height: 20px;
	width: 20px;

	box-shadow: 0 0 0 0 rgba(255, 82, 82, 1);
	transform: scale(1);
	animation: pulse 2s infinite;
}

@keyframes flash {
  0% {
    border-color: transparent;
  }
  100% {
    border-color: #FF4136;
  }
}

@keyframes pulse {
	0% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0.7);
	}
	
	70% {
		transform: scale(1);
		box-shadow: 0 0 0 10px rgba(255, 82, 82, 0);
	}
	
	100% {
		transform: scale(0.95);
		box-shadow: 0 0 0 0 rgba(255, 82, 82, 0);
	}
}
</style>