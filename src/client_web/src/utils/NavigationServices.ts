import router from '@/router'
import type { SelectionMenu } from './NavigationGenerator'

class NavMenu {
  mainpath = '' as string
  subpath = '' as string
  requiresAuth = true as boolean
  mainmenu = true as boolean
  title = '' as string
  groupmenu = '' as string
  groupmenuicon = '' as string
  nameTh = '' as string
  icon = '' as string
}

export function GeneratedAllMenu() {
  const NavMenuList: any = []
  router.options.routes.forEach((route) => {
    if (route.children) {
      let childrens = route.children
      childrens.forEach((data) => {
        if (data.meta?.mainmenu) {
          const newNavMenu = new NavMenu()
          newNavMenu.mainpath = route.path
          ;(newNavMenu.subpath = data.path),
            (newNavMenu.requiresAuth = data.meta.requiresAuth ? true : false),
            (newNavMenu.mainmenu = data.meta.mainmenu ? true : false),
            (newNavMenu.title = data.meta.title as string),
            (newNavMenu.groupmenu = data.meta.groupmenu as string),
            (newNavMenu.groupmenuicon = data.meta.groupmenuicon as string),
            (newNavMenu.nameTh = data.meta.nameTh as string),
            (newNavMenu.icon = data.meta.icon as string)
          if (newNavMenu.mainmenu == true) {
            NavMenuList.push(newNavMenu)
          }
        }
      })
    }
  })
  return GroupMenu(NavMenuList)
}

export function GeneratedIncludeMenu(selectionMenu: SelectionMenu) {
  const NavMenuList: any = []
  router.options.routes.forEach((route) => {
    if (route.children) {
      let childrens = route.children
      childrens.forEach((data) => {
        if (data.meta?.mainmenu) {
          const newNavMenu = new NavMenu()
          newNavMenu.mainpath = route.path
          ;(newNavMenu.subpath = data.path),
            (newNavMenu.requiresAuth = data.meta.requiresAuth ? true : false),
            (newNavMenu.mainmenu = data.meta.mainmenu ? true : false),
            (newNavMenu.title = data.meta.title as string),
            (newNavMenu.groupmenu = data.meta.groupmenu as string),
            (newNavMenu.groupmenuicon = data.meta.groupmenuicon as string),
            (newNavMenu.nameTh = data.meta.nameTh as string),
            (newNavMenu.icon = data.meta.icon as string)
          if (
            newNavMenu.mainmenu == true &&
            selectionMenu.Menu.find((x) => x.menuName == newNavMenu.nameTh)
          ) {
            NavMenuList.push(newNavMenu)
          }
        }
      })
    }
  })
  return OrderBy(GroupMenu(NavMenuList), selectionMenu)
}

export function GeneratedExcludeMenu(includes: string[]) {
  const NavMenuList: any = []
  router.options.routes.forEach((route) => {
    if (route.children) {
      let childrens = route.children
      childrens.forEach((data) => {
        if (data.meta?.mainmenu) {
          const newNavMenu = new NavMenu()
          newNavMenu.mainpath = route.path
          ;(newNavMenu.subpath = data.path),
            (newNavMenu.requiresAuth = data.meta.requiresAuth ? true : false),
            (newNavMenu.mainmenu = data.meta.mainmenu ? true : false),
            (newNavMenu.title = data.meta.title as string),
            (newNavMenu.groupmenu = data.meta.groupmenu as string),
            (newNavMenu.groupmenuicon = data.meta.groupmenuicon as string),
            (newNavMenu.nameTh = data.meta.nameTh as string),
            (newNavMenu.icon = data.meta.icon as string)
          if (newNavMenu.mainmenu == true && !includes.includes(newNavMenu.nameTh)) {
            NavMenuList.push(newNavMenu)
          }
        }
      })
    }
  })
  return GroupMenu(NavMenuList)
}

export function OrderBy(data: GroupedNavMenu[], selectionMenu: SelectionMenu): GroupedNavMenu[] {
  if (selectionMenu.Menu.length > 0) {
    // Sort by Menu, then Group, then Title as fallback
    data.forEach((titleItem) => {
      titleItem.titles.forEach((groupItem) => {
        groupItem.menus.sort((a, b) => {
          const menuA = selectionMenu.Menu.find((m) => m.menuName === a.nameTh)
          const menuB = selectionMenu.Menu.find((m) => m.menuName === b.nameTh)
          const orderA = menuA ? menuA.menuOrder : Infinity // If not found in selectionMenu, place it last
          const orderB = menuB ? menuB.menuOrder : Infinity
          return orderA - orderB
        })
      })
    })

    //Sort Groups within each title based on groupOrder if available, otherwise maintain original order
    data.forEach((titleItem) => {
      titleItem.titles.sort((a, b) => {
        const groupA = selectionMenu.Group.find((g) => g.groupName === a.groupmenu)
        const groupB = selectionMenu.Group.find((g) => g.groupName === b.groupmenu)

        const orderA = groupA ? groupA.groupOrder : Infinity
        const orderB = groupB ? groupB.groupOrder : Infinity

        return orderA - orderB
      })
    })

    // Sort Titles based on titleOrder if available, otherwise maintain original order.
    data.sort((a, b) => {
      const titleA = selectionMenu.Title.find((t) => t.titleName === a.title)
      const titleB = selectionMenu.Title.find((t) => t.titleName === b.title)
      const orderA = titleA ? titleA.titleOrder : Infinity
      const orderB = titleB ? titleB.titleOrder : Infinity
      return orderA - orderB
    })
  } else if (selectionMenu.Group.length > 0) {
    // Sort by Group, then Title as fallback
    data.forEach((titleItem) => {
      titleItem.titles.sort((a, b) => {
        const groupA = selectionMenu.Group.find((g) => g.groupName === a.groupmenu)
        const groupB = selectionMenu.Group.find((g) => g.groupName === b.groupmenu)
        const orderA = groupA ? groupA.groupOrder : Infinity
        const orderB = groupB ? groupB.groupOrder : Infinity
        return orderA - orderB
      })
    })

    // Apply Title sorting as a fallback if no menu sorting is provided.
    data.sort((a, b) => {
      const titleA = selectionMenu.Title.find((t) => t.titleName === a.title)
      const titleB = selectionMenu.Title.find((t) => t.titleName === b.title)
      const orderA = titleA ? titleA.titleOrder : Infinity
      const orderB = titleB ? titleB.titleOrder : Infinity
      return orderA - orderB
    })
  } else if (selectionMenu.Title.length > 0) {
    // Sort by Title only
    data.sort((a, b) => {
      const titleA = selectionMenu.Title.find((t) => t.titleName === a.title)
      const titleB = selectionMenu.Title.find((t) => t.titleName === b.title)
      const orderA = titleA ? titleA.titleOrder : Infinity
      const orderB = titleB ? titleB.titleOrder : Infinity
      return orderA - orderB
    })
  }

  return data
}

export type GroupedNavMenu = {
  title: string
  titles: {
    groupmenu: string
    groupmenuicon: string
    menus: {
      mainpath: string
      subpath: string
      requiresAuth: boolean
      icon: string
      nameTh: string
    }[]
  }[]
}
// Function to group NavMenu data
export function GroupMenu(data: NavMenu[]): GroupedNavMenu[] {
  const grouped: GroupedNavMenu[] = []

  // Group by title first
  const groupedByTitle = data.reduce(
    (acc, item) => {
      acc[item.title] = acc[item.title] || []
      acc[item.title].push(item)
      return acc
    },
    {} as Record<string, NavMenu[]>,
  )

  for (const title in groupedByTitle) {
    const items = groupedByTitle[title]

    // Now group by groupmenu within each title group
    const groupedByGroupMenu = items.reduce(
      (acc, item) => {
        const key = item.groupmenu //  + "|" + item.groupmenuicon; // If you need to also group by the icon

        acc[key] = acc[key] || {
          groupmenu: item.groupmenu,
          groupmenuicon: item.groupmenuicon,
          menus: [],
        }
        acc[key].menus.push({
          mainpath: item.mainpath,
          subpath: item.subpath,
          requiresAuth: item.requiresAuth,
          icon: item.icon,
          nameTh: item.nameTh,
        })
        return acc
      },
      {} as Record<string, { groupmenu: string; groupmenuicon: string; menus: any[] }>,
    )

    grouped.push({
      title: title,
      titles: Object.values(groupedByGroupMenu),
    })
  }

  return grouped
}
