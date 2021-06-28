package be.kdg.stemtest.DI.modules.fragmentModules

import be.kdg.stemtest.view.fragments.MoreInfo
import dagger.Module
import dagger.android.ContributesAndroidInjector

@Module
abstract class MoreInfoFragmentModule {

    @ContributesAndroidInjector
    abstract fun bindMoreInfoFragment(): MoreInfo
}