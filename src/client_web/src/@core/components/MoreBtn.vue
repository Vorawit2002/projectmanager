<script lang="ts" setup>
const emit = defineEmits(['Types'])

function reSendValue(value: any) {
  emit('Types', value)
}
interface Props {
  menuList?: unknown[]
  itemProps?: boolean
  iconSize?: string
}

const props = defineProps<Props>()
</script>

<template>
  <IconBtn>
    <VIcon
      :size="iconSize"
      icon="ri-more-2-line"
    />

     <VMenu v-if="props.menuList" activator="parent">
      <VList>
        <VListItem
          v-for="item in props.menuList"
          :key="item.value"
          @click="reSendValue(item.value)"
        >
          <VListItemTitle>{{ item.title }}</VListItemTitle>
        </VListItem>
      </VList>
    </VMenu>
  </IconBtn>
</template>
