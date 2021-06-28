package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.Waitingscreen
import dagger.Module
import dagger.android.ContributesAndroidInjector


@Module
abstract class WachtschermFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindWachtschermFragment(): Waitingscreen
}