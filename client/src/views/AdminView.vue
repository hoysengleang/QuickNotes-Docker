<template>
  <div class="p-10 bg-gray-50 min-h-screen">
    <h1 class="text-3xl font-bold text-purple-600">🛡️ Admin Panel</h1>
    <p class="mt-2">Only logged-in users can see this.</p>
    <button @click="$router.push('/')" class="mt-4 text-blue-600 underline">Back Home</button>
  </div>
</template>