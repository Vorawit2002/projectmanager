export class BaseClass {
    /**
     * authorization token value
     */
    private static isRedirecting = false
    private static unauthorizedCallbacks: Array<() => void> = []

    constructor() {}
  
    protected transformOptions(options: any): Promise<any> { 
        const token = localStorage.getItem('TOKEN_KEY')
        
        if (token) {
            // Add JWT token to Authorization header with proper Bearer format
            options.headers["Authorization"] = `Bearer ${token}`
        } else {
            // Only warn for non-public endpoints
            const url = options.url || ''
            const isPublicEndpoint = url.includes('/login') || 
                                    url.includes('/register') || 
                                    url.includes('/refresh-token')
            
            if (!isPublicEndpoint) {
                console.warn("Authorization token not found. User may need to login.")
            }
        }
        
        // Add response interceptor by wrapping the fetch
        const originalFetch = options.fetch || window.fetch.bind(window)
        options.fetch = async (url: RequestInfo, init?: RequestInit): Promise<Response> => {
            try {
                const response = await originalFetch(url, init)
                
                // Handle 401 Unauthorized responses
                if (response.status === 401 && !BaseClass.isRedirecting) {
                    BaseClass.handleUnauthorized()
                }
                
                return response
            } catch (error) {
                throw error
            }
        }
        
        return Promise.resolve(options)
    }

    /**
     * Handle 401 Unauthorized responses
     */
    private static handleUnauthorized(): void {
        if (BaseClass.isRedirecting) {
            return
        }
        
        BaseClass.isRedirecting = true
        
        // Clear token
        localStorage.removeItem('TOKEN_KEY')
        
        // Execute callbacks (for store cleanup, etc.)
        BaseClass.unauthorizedCallbacks.forEach(callback => {
            try {
                callback()
            } catch (error) {
                console.error('Error in unauthorized callback:', error)
            }
        })
        
        // Redirect to login page
        console.warn('Session expired or unauthorized. Redirecting to login...')
        
        // Use setTimeout to avoid blocking
        setTimeout(() => {
            window.location.href = '/login'
            BaseClass.isRedirecting = false
        }, 100)
    }

    /**
     * Register a callback to be called when 401 Unauthorized is received
     */
    public static onUnauthorized(callback: () => void): void {
        BaseClass.unauthorizedCallbacks.push(callback)
    }

    /**
     * Clear all unauthorized callbacks
     */
    public static clearUnauthorizedCallbacks(): void {
        BaseClass.unauthorizedCallbacks = []
    }
}
