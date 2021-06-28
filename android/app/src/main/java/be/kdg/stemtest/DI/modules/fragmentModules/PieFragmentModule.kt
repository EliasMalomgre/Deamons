package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.customdebategame.PieFragment
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class PieFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindPielFragment(): PieFragment
}